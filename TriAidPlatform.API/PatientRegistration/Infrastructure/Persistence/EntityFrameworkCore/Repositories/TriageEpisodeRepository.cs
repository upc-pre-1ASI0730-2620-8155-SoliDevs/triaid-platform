using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.PatientRegistration.Domain.Model.Aggregates;
using TriAidPlatform.API.PatientRegistration.Domain.Model.ValueObjects;
using TriAidPlatform.API.PatientRegistration.Domain.Repositories;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace TriAidPlatform.API.PatientRegistration.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
/// Infrastructure implementation of the <see cref="ITriageEpisodeRepository"/> interface.
/// Provides data access operations for the <see cref="TriageEpisode"/> aggregate using Entity Framework Core.
/// </summary>
/// <param name="context">The database context provided via dependency injection.</param>
public class TriageEpisodeRepository(AppDbContext context)
    : BaseRepository<TriageEpisode>(context), ITriageEpisodeRepository
{
    /// <inheritdoc />
    public async Task<IEnumerable<TriageEpisode>> FindByPatientIdAsync(
        int patientId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<TriageEpisode>()
            .Where(e => e.PatientId == patientId)
            .OrderByDescending(e => e.ArrivalTime)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsOpenByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<TriageEpisode>()
            .AnyAsync(e => e.PatientId == patientId
                           && (e.State == EEpisodeState.Waiting || e.State == EEpisodeState.InTriage),
                cancellationToken);
    }
}
