using TriAidPlatform.API.Devices.Domain.Model.Aggregates;

namespace TriAidPlatform.API.Devices.Domain.Repositories;

/// <summary>
/// Repository interface for <see cref="Device"/> aggregate root operations.
/// </summary>
public interface IDeviceRepository
{
    /// <summary>Finds a device by its unique identifier.</summary>
    Task<Device?> FindByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>Finds a device by its hardware serial number.</summary>
    Task<Device?> FindBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken);

    /// <summary>Retrieves all registered devices.</summary>
    Task<IEnumerable<Device>> ListAsync(CancellationToken cancellationToken);

    /// <summary>Adds a new device to the persistence store.</summary>
    Task AddAsync(Device device, CancellationToken cancellationToken);

    /// <summary>Updates an existing device in the persistence store.</summary>
    void Update(Device device);

    /// <summary>Removes a device from the persistence store.</summary>
    void Remove(Device device);
}