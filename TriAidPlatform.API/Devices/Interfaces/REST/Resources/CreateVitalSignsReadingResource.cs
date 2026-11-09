namespace TriAidPlatform.API.Devices.Interfaces.REST.Resources;

/// <summary>
/// Resource representation for creating a Vital Signs Reading.
/// </summary>
/// <param name="DeviceId">The device ID that captured the reading.</param>
/// <param name="EpisodeId">The clinical episode ID.</param>
/// <param name="SystolicPressure">Systolic blood pressure.</param>
/// <param name="DiastolicPressure">Diastolic blood pressure.</param>
/// <param name="HeartRate">Heart rate.</param>
/// <param name="Temperature">Body temperature.</param>
/// <param name="OxygenSaturation">Oxygen saturation level.</param>
public record CreateVitalSignsReadingResource(
    int DeviceId,
    int EpisodeId,
    int SystolicPressure,
    int DiastolicPressure,
    int HeartRate,
    decimal Temperature,
    int OxygenSaturation
);