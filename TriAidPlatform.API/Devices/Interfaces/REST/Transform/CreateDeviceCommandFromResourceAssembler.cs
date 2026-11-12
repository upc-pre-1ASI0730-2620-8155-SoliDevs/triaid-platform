using TriAidPlatform.API.Devices.Domain.Model.Commands;
using TriAidPlatform.API.Devices.Interfaces.REST.Resources;

namespace TriAidPlatform.API.Devices.Interfaces.REST.Transform;

/// <summary>Builds a <see cref="CreateDeviceCommand"/> from the device record sent by the frontend.</summary>
public static class CreateDeviceCommandFromResourceAssembler
{
    /// <summary>Maps the create resource to its command.</summary>
    /// <param name="resource">Device record sent by the frontend.</param>
    /// <returns>The create device command.</returns>
    /// <exception cref="ArgumentException">Thrown when the type or the last use is not valid.</exception>
    public static CreateDeviceCommand ToCommandFromResource(CreateDeviceResource resource)
    {
        return new CreateDeviceCommand(
            resource.SerialNumber,
            DeviceTypeKeyAssembler.ToDeviceType(resource.Type),
            resource.Model,
            resource.Online,
            LastUseAssembler.ToDateTime(resource.LastUse),
            resource.Institution);
    }
}
