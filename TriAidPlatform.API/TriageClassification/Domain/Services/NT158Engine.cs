using System.Text.RegularExpressions;
using TriAidPlatform.API.TriageClassification.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.TriageClassification.Domain.Services;

/// <summary>
/// Classification outcome of the NT-158 evaluation: the most urgent level
/// matched by any rule, plus the metrics that are still missing when the
/// mandatory inputs are incomplete (US19 - Escenario 2).
/// </summary>
/// <param name="Level">Suggested triage level, or null when classification is not possible.</param>
/// <param name="MissingMetrics">Mandatory metric names that were not provided.</param>
public sealed record ClassificationResult(ETriageLevel? Level, IReadOnlyList<string> MissingMetrics)
{
    /// <summary>Whether the evaluation produced a usable suggestion.</summary>
    public bool HasSuggestion => Level is not null;
}

/// <summary>
/// Domain service that computes the suggested triage priority from the
/// episode vital signs using the normative ranges of Health Technical
/// Standard No. 158-MINSA/DIGEPRES. Each rule evaluates one metric and
/// contributes its level as a candidate; the most urgent candidate wins.
/// </summary>
public static partial class NT158Engine
{
    /// <summary>Metric names required to produce a suggestion (US19 - Escenario 2).</summary>
    public static readonly string[] RequiredMetrics = ["spo2", "fc", "pa"];

    /// <summary>Normative candidate rule per metric and level (US19 - Escenario 1).</summary>
    private static readonly (string Metric, ETriageLevel Level, Func<decimal, bool> Test)[] Rules =
    [
        // Level I - Resuscitation
        ("spo2", ETriageLevel.Resuscitation, v => v < 90),
        ("fc", ETriageLevel.Resuscitation, v => v < 40 || v > 150),
        ("pas", ETriageLevel.Resuscitation, v => v < 80 || v > 220),
        ("temp", ETriageLevel.Resuscitation, v => v < 35 || v > 41),

        // Level II - Emergency
        ("spo2", ETriageLevel.Emergency, v => v is >= 90 and <= 93),
        ("fc", ETriageLevel.Emergency, v => v is >= 40 and <= 50 or >= 121 and <= 150),
        ("pas", ETriageLevel.Emergency, v => v is >= 80 and <= 89 or >= 181 and <= 220),
        ("temp", ETriageLevel.Emergency, v => v is >= 35 and < 36 or > 39 and <= 41),

        // Level III - Urgent
        ("spo2", ETriageLevel.Urgent, v => v is >= 94 and <= 95),
        ("fc", ETriageLevel.Urgent, v => v is >= 51 and <= 60 or >= 101 and <= 120),
        ("pas", ETriageLevel.Urgent, v => v is >= 90 and <= 99 or >= 141 and <= 180),
        ("temp", ETriageLevel.Urgent, v => v is > 38 and <= 39)
    ];

    [GeneratedRegex("/")]
    private static partial Regex SlashSeparator();

    /// <summary>Parses a decimal metric value accepting comma decimal separators.</summary>
    private static decimal? ParseMetric(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;
        return decimal.TryParse(raw.Trim().Replace(',', '.'), out var value) ? value : null;
    }

    /// <summary>
    /// Flattens the episode vital sign readings into the numeric summary
    /// the rules evaluate. Blood pressure arrives as "systolic/diastolic".
    /// </summary>
    /// <param name="readings">Map of metric name to reading value.</param>
    /// <returns>The numeric summary of the available metrics.</returns>
    public static VitalSignsSummary Summarize(IReadOnlyDictionary<string, string> readings)
    {
        string? raw(string key) => readings.TryGetValue(key, out var value) ? value : null;
        var pressure = (raw("pa") ?? string.Empty).Split('/');
        return new VitalSignsSummary(
            ParseMetric(raw("spo2")),
            ParseMetric(raw("fc")),
            ParseMetric(pressure.Length > 0 ? pressure[0] : null),
            ParseMetric(pressure.Length > 1 ? pressure[1] : null),
            ParseMetric(raw("temp")));
    }

    /// <summary>
    /// Runs the NT-158 rules over the vital signs summary (US19).
    /// Reports the missing mandatory metrics instead of guessing when the
    /// respiratory inputs are incomplete.
    /// </summary>
    /// <param name="summary">Numeric vital signs of the episode.</param>
    /// <returns>The suggested level or the list of missing metrics.</returns>
    public static ClassificationResult Evaluate(VitalSignsSummary summary)
    {
        var missing = new List<string>();
        if (summary.SpO2 is null) missing.Add("spo2");
        if (summary.HeartRate is null) missing.Add("fc");
        if (summary.Systolic is null) missing.Add("pa");
        if (missing.Count > 0) return new ClassificationResult(null, missing);

        ETriageLevel? mostUrgent = null;
        foreach (var (metric, level, test) in Rules)
        {
            var value = metric switch
            {
                "spo2" => summary.SpO2,
                "fc" => summary.HeartRate,
                "pas" => summary.Systolic,
                "temp" => summary.Temperature,
                _ => null
            };
            if (value is not null && test(value.Value) &&
                (mostUrgent is null || level < mostUrgent)) mostUrgent = level;
        }

        return mostUrgent is null
            ? new ClassificationResult(ETriageLevel.LessUrgent, Array.Empty<string>())
            : new ClassificationResult(mostUrgent, Array.Empty<string>());
    }
}
