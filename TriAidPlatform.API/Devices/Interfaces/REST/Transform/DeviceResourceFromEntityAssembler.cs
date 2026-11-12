using TriAidPlatform.API.Devices.Domain.Model.Aggregates;
using TriAidPlatform.API.Devices.Interfaces.REST.Resources;

namespace TriAidPlatform.API.Devices.Interfaces.REST.Transform;

/// <summary>Transforms the Device aggregate into its REST resource.</summary>
public static class DeviceResourceFromEntityAssembler
{
    /// <summary>Maps the aggregate state to the REST representation.</summary>
    /// <param name="entity">Aggregate to transform.</param>
    /// <returns>The REST resource.</returns>
    public static DeviceResource ToResourceFromEntity(Device entity)
    {
        return new DeviceResource(
            entity.Id,
            entity.SerialNumber,
            entity.Model,
            entity.Type.ToString(),
            entity.IsActive,
            entity.RegisteredAt
        );
    }
}
