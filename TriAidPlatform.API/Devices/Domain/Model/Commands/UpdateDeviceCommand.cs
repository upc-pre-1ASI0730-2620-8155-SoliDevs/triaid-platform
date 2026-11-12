using TriAidPlatform.API.Devices.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.Devices.Domain.Model.Commands;

/// <summary>Command that replaces the full record of a device.</summary>
/// <param name="DeviceId">Identifier of the device.</param>
/// <param name="SerialNumber">Hardware serial number, when it is known.</param>
/// <param name="Type">Type of the device.</param>
/// <param name="Model">Commercial model of the device.</param>
/// <param name="Online">Whether the device is online.</param>
/// <param name="LinkedEpisodeId">Triage episode the device is linked to, or null to release it.</param>
/// <param name="LinkedAt">Moment of the link, if the client sends it.</param>
/// <param name="LastUse">Moment of the last reading taken with the device, if any.</param>
/// <param name="Institution">Institution that owns the device.</param>
public record UpdateDeviceCommand(
    int DeviceId,
    string? SerialNumber,
    EDeviceType Type,
    string Model,
    bool Online,
    string? LinkedEpisodeId,
    DateTime? LinkedAt,
    DateTime? LastUse,
    string? Institution);
