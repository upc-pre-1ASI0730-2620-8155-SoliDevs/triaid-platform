namespace TriAidPlatform.API.Devices.Interfaces.REST.Resources;

/// <summary>
/// Resource representation for creating a Device.
/// </summary>
/// <param name="SerialNumber">The serial number.</param>
/// <param name="Model">The device model.</param>
/// <param name="DeviceType">The type of the device.</param>
public record CreateDeviceResource(
    string SerialNumber,
    string Model,
    string DeviceType
);