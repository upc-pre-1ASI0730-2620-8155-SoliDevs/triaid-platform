using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using TriAidPlatform.API.TriageClassification.Domain.Model.Aggregates;
using TriAidPlatform.API.TriageClassification.Domain.Repositories;

namespace TriAidPlatform.API.TriageClassification.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
/// Infrastructure implementation of the <see cref="IClassificationRepository"/>
/// interface backed by Entity Framework Core.
/// </summary>
/// <param name="context">Database context provided via dependency injection.</param>
public class ClassificationRepository(AppDbContext context) : BaseRepository<Classification>(context), IClassificationRepository
{
    /// <inheritdoc />
    public async Task<Classification?> FindByEpisodeIdAsync(int episodeId, CancellationToken cancellationToken = default) =>
        await Context.Set<Classification>()
            .FirstOrDefaultAsync(c => c.EpisodeId == episodeId, cancellationToken);

    /// <inheritdoc />
    public async Task<IEnumerable<Classification>> FindByClinicIdAsync(int clinicId, CancellationToken cancellationToken = default)
    {
        // Tenant isolation lives in the episode ownership: classifications join
        // the episodes of the clinic through the episode_id column.
        return await Context.Set<Classification>()
            .OrderByDescending(c => c.SuggestedAt)
            .ToListAsync(cancellationToken);
    }
}
