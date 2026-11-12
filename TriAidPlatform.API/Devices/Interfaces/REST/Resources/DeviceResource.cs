namespace TriAidPlatform.API.Devices.Interfaces.REST.Resources;

/// <summary>
/// Resource representation of a Device, aligned with the device record of the frontend.
/// </summary>
/// <param name="Id">The unique identifier of the device.</param>
/// <param name="Type">The device type key (pa, spo2, fc, temp).</param>
/// <param name="Model">The device model.</param>
/// <param name="Online">Indicates if the device is online.</param>
/// <param name="LinkedEpisode">Triage episode the device is linked to, if any.</param>
/// <param name="LinkedAt">Moment the device was linked to the episode.</param>
/// <param name="LastUse">Time (HH:mm) of the last reading taken with the device.</param>
/// <param name="Institution">Institution that owns the device.</param>
/// <param name="SerialNumber">The serial number, when it is known.</param>
public record DeviceResource(
    int Id,
    string Type,
    string Model,
    bool Online,
    string? LinkedEpisode,
    DateTime? LinkedAt,
    string? LastUse,
    string Institution,
    string? SerialNumber
);