using TriAidPlatform.API.Devices.Domain.Model.Aggregates;
using TriAidPlatform.API.Devices.Domain.Model.ValueObjects;
using TriAidPlatform.API.Devices.Interfaces.REST.Resources;

namespace TriAidPlatform.API.Devices.Interfaces.REST.Transform;

public static class VitalSignsReadingAssembler
{
    public static VitalSignsReading ToModelFromCreate(CreateVitalSignsReadingResource resource, EDeviceType deviceType)
    {
        
        return new VitalSignsReading(
            resource.EpisodeId,
            resource.DeviceId,
            deviceType,
            resource.HeartRate,
            resource.OxygenSaturation,
            resource.SystolicPressure,
            resource.DiastolicPressure,
            resource.Temperature
        );
    }

    public static VitalSignsReadingResource ToResourceFromEntity(VitalSignsReading entity)
    {
        return new VitalSignsReadingResource(
            entity.Id,
            entity.DeviceId,
            entity.EpisodeId,
            entity.SystolicBloodPressure,
            entity.DiastolicBloodPressure,
            entity.HeartRate,
            entity.Temperature,
            entity.OxygenSaturation,
            entity.CapturedAt
        );
    }
}