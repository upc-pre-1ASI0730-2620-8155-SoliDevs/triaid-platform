using TriAidPlatform.API.Devices.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.Devices.Domain.Model.Commands;

/// <summary>Command that registers a device in the inventory of an institution.</summary>
/// <param name="SerialNumber">Hardware serial number, when it is known.</param>
/// <param name="Type">Type of the device.</param>
/// <param name="Model">Commercial model of the device.</param>
/// <param name="Online">Whether the device is online.</param>
/// <param name="LastUse">Moment of the last reading taken with the device, if any.</param>
/// <param name="Institution">Institution that owns the device.</param>
public record CreateDeviceCommand(
    string? SerialNumber,
    EDeviceType Type,
    string Model,
    bool Online,
    DateTime? LastUse,
    string? Institution);
