using TriAidPlatform.API.TriageClassification.Domain.Services;
using TriAidPlatform.API.TriageClassification.Interfaces.REST.Resources;

namespace TriAidPlatform.API.TriageClassification.Interfaces.REST.Transform;

/// <summary>Transforms the suggestion request into the engine input summary.</summary>
public static class SuggestCommandFromResourceAssembler
{
    /// <summary>Flattens the REST payload into the vital signs summary.</summary>
    /// <param name="resource">Request payload.</param>
    /// <returns>Map of metric names to raw values.</returns>
    public static IReadOnlyDictionary<string, string> ToReadingsMap(SuggestClassificationResource resource) =>
        new Dictionary<string, string>
        {
            ["spo2"] = resource.SpO2 ?? string.Empty,
            ["fc"] = resource.HeartRate ?? string.Empty,
            ["pa"] = resource.BloodPressure ?? string.Empty,
            ["temp"] = resource.Temperature ?? string.Empty
        };
}
