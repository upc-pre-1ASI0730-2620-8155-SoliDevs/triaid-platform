namespace TriAidPlatform.API.TriageClassification.Domain.Model.ValueObjects;

/// <summary>
/// Value object that flattens the vital signs of an episode into the
/// numeric inputs of the NT-158 classification rules.
/// </summary>
/// <param name="SpO2">Peripheral oxygen saturation in percent.</param>
/// <param name="HeartRate">Heart rate in beats per minute.</param>
/// <param name="Systolic">Systolic blood pressure in mmHg.</param>
/// <param name="Diastolic">Diastolic blood pressure in mmHg.</param>
/// <param name="Temperature">Body temperature in degrees Celsius.</param>
public sealed record VitalSignsSummary(
    decimal? SpO2,
    decimal? HeartRate,
    decimal? Systolic,
    decimal? Diastolic,
    decimal? Temperature)
{
    /// <summary>Systolic blood pressure (alias kept for rule readability).</summary>
    public decimal? Pas => Systolic;
    public decimal? Pad => Diastolic;
}
