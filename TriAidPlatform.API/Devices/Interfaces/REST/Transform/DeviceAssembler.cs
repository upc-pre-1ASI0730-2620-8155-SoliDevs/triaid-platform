using TriAidPlatform.API.Devices.Domain.Model.Aggregates;
using TriAidPlatform.API.Devices.Domain.Model.ValueObjects;
using TriAidPlatform.API.Devices.Interfaces.REST.Resources;

namespace TriAidPlatform.API.Devices.Interfaces.REST.Transform;

public static class DeviceAssembler
{
    public static Device ToModelFromCreate(CreateDeviceResource resource)
    {
        if (!Enum.TryParse<EDeviceType>(resource.DeviceType, true, out var deviceType))
        {
            throw new ArgumentException($"Invalid device type: '{resource.DeviceType}'.");
        }

        
        return new Device(resource.SerialNumber, deviceType, resource.Model);
    }
}