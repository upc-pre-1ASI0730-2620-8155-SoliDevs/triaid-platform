namespace TriAidPlatform.API.TriageClassification.Interfaces.REST.Resources;

/// <summary>Request body to record the NT-158 suggestion of an episode (US19).</summary>
/// <param name="SpO2">Peripheral oxygen saturation in percent.</param>
/// <param name="HeartRate">Heart rate in beats per minute.</param>
/// <param name="BloodPressure">Blood pressure as "systolic/diastolic" mmHg.</param>
/// <param name="Temperature">Body temperature in degrees Celsius.</param>
public record SuggestClassificationResource(string? SpO2, string? HeartRate, string? BloodPressure, string? Temperature);
