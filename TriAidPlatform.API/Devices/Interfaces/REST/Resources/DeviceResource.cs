namespace TriAidPlatform.API.Devices.Interfaces.REST.Resources;

/// <summary>
/// Resource representation of a Device.
/// </summary>
/// <param name="Id">The unique identifier of the device.</param>
/// <param name="SerialNumber">The serial number.</param>
/// <param name="Model">The device model.</param>
/// <param name="DeviceType">The type of the device.</param>
/// <param name="IsActive">Indicates if the device is active.</param>
/// <param name="RegisteredAt">Registration timestamp.</param>
public record DeviceResource(
    int Id,
    string SerialNumber,
    string Model,
    string DeviceType,
    bool IsActive,
    DateTime RegisteredAt
);