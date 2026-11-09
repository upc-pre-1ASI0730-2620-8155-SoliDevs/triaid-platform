using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.Devices.Domain.Model.Aggregates;
using TriAidPlatform.API.Devices.Domain.Repositories;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace TriAidPlatform.API.Devices.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
/// Entity Framework Core implementation of <see cref="IVitalSignsReadingRepository"/>.
/// </summary>
public class VitalSignsReadingRepository : BaseRepository<AppDbContext>, IVitalSignsReadingRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VitalSignsReadingRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public VitalSignsReadingRepository(AppDbContext context) : base(context)
    {
    }

    /// <inheritdoc />
    public async Task<VitalSignsReading?> FindByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await Context.Set<VitalSignsReading>().FindAsync([id], cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<VitalSignsReading>> ListByEpisodeIdAsync(int episodeId, CancellationToken cancellationToken)
    {
        return await Context.Set<VitalSignsReading>()
            .Where(r => r.EpisodeId == episodeId)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<VitalSignsReading>> ListAsync(CancellationToken cancellationToken)
    {
        return await Context.Set<VitalSignsReading>().ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(VitalSignsReading reading, CancellationToken cancellationToken)
    {
        await Context.Set<VitalSignsReading>().AddAsync(reading, cancellationToken);
    }
}