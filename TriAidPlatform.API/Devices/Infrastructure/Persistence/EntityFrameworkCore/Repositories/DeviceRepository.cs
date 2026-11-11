using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.Devices.Domain.Model.Aggregates;
using TriAidPlatform.API.Devices.Domain.Repositories;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace TriAidPlatform.API.Devices.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
/// Entity Framework Core implementation of <see cref="IDeviceRepository"/>.
/// </summary>
public class DeviceRepository : BaseRepository<AppDbContext>, IDeviceRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public DeviceRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<Device?> FindByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await Context.Set<Device>().FindAsync([id], cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Device?> FindBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken)
    {
        return await Context.Set<Device>()
            .FirstOrDefaultAsync(d => d.SerialNumber == serialNumber, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Device>> ListAsync(CancellationToken cancellationToken)
    {
        return await Context.Set<Device>().ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(Device device, CancellationToken cancellationToken)
    {
        await Context.Set<Device>().AddAsync(device, cancellationToken);
    }

    /// <inheritdoc />
    public void Update(Device device)
    {
        Context.Set<Device>().Update(device);
    }

    /// <inheritdoc />
    public void Remove(Device device)
    {
        Context.Set<Device>().Remove(device);
    }
}