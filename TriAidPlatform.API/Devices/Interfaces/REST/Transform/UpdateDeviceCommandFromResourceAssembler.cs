using TriAidPlatform.API.Devices.Domain.Model.Commands;
using TriAidPlatform.API.Devices.Interfaces.REST.Resources;

namespace TriAidPlatform.API.Devices.Interfaces.REST.Transform;

/// <summary>Builds an <see cref="UpdateDeviceCommand"/> from the route and the device record sent by the frontend.</summary>
public static class UpdateDeviceCommandFromResourceAssembler
{
    /// <summary>Maps the update resource to its command.</summary>
    /// <param name="deviceId">Identifier of the device taken from the route.</param>
    /// <param name="resource">Full device record sent by the frontend.</param>
    /// <returns>The update device command.</returns>
    /// <exception cref="ArgumentException">Thrown when the type or the last use is not valid.</exception>
    public static UpdateDeviceCommand ToCommandFromResource(int deviceId, UpdateDeviceResource resource)
    {
        return new UpdateDeviceCommand(
            deviceId,
            resource.SerialNumber,
            DeviceTypeKeyAssembler.ToDeviceType(resource.Type),
            resource.Model,
            resource.Online,
            resource.LinkedEpisode,
            resource.LinkedAt,
            LastUseAssembler.ToDateTime(resource.LastUse),
            resource.Institution);
    }
}
