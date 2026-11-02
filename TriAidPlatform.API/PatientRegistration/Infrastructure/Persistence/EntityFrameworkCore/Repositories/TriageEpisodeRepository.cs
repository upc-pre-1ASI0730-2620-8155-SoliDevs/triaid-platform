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
    /// <summary>
    /// Retrieves every triage episode of a patient, ordered from the most recent arrival.
    /// </summary>
    /// <param name="patientId">Identifier of the patient.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>An asynchronous task returning the episodes of the patient.</returns>
    public async Task<IEnumerable<TriageEpisode>> FindByPatientIdAsync(
        int patientId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<TriageEpisode>()
            .Where(e => e.PatientId == patientId)
            .OrderByDescending(e => e.ArrivalTime)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Reports whether the patient already has an episode in progress (Waiting or InTriage).
    /// Used to prevent opening duplicated episodes for the same patient.
    /// </summary>
    /// <param name="patientId">Identifier of the patient.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>An asynchronous task returning true when an open episode exists; otherwise, false.</returns>
    public async Task<bool> ExistsOpenByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<TriageEpisode>()
            .AnyAsync(e => e.PatientId == patientId
                           && (e.State == EEpisodeState.Waiting || e.State == EEpisodeState.InTriage),
                cancellationToken);
    }
}
