namespace TriAidPlatform.API.Devices.Interfaces.REST.Resources;

/// <summary>
/// Resource representation for creating a Device, as sent by the frontend.
/// </summary>
/// <param name="Type">The device type key (pa, spo2, fc, temp).</param>
/// <param name="Model">The device model.</param>
/// <param name="Online">Indicates if the device is online.</param>
/// <param name="LastUse">Time (HH:mm) of the last reading, or empty when never used.</param>
/// <param name="Institution">Institution that owns the device.</param>
/// <param name="SerialNumber">The serial number (optional).</param>
public record CreateDeviceResource(
    string Type,
    string Model,
    bool Online,
    string? LastUse,
    string? Institution,
    string? SerialNumber
);