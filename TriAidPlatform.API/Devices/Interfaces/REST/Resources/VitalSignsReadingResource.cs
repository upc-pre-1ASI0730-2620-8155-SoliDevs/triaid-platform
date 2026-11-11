namespace TriAidPlatform.API.Devices.Interfaces.REST.Resources;

/// <summary>
/// Resource representation of a Vital Signs Reading.
/// </summary>
/// <param name="Id">The unique identifier.</param>
/// <param name="DeviceId">The device ID that captured the reading.</param>
/// <param name="EpisodeId">The clinical episode ID.</param>
/// <param name="SystolicPressure">Systolic blood pressure.</param>
/// <param name="DiastolicPressure">Diastolic blood pressure.</param>
/// <param name="HeartRate">Heart rate.</param>
/// <param name="Temperature">Body temperature.</param>
/// <param name="OxygenSaturation">Oxygen saturation level.</param>
/// <param name="RecordedAt">Timestamp of the reading.</param>
public record VitalSignsReadingResource(
    int Id,
    int DeviceId,
    int EpisodeId,
    int SystolicPressure,
    int DiastolicPressure,
    int HeartRate,
    decimal Temperature,
    int OxygenSaturation,
    DateTime RecordedAt
);