using TriAidPlatform.API.PatientRegistration.Domain.Model.Aggregates;
using TriAidPlatform.API.Shared.Domain.Repositories;

namespace TriAidPlatform.API.PatientRegistration.Domain.Repositories;

/// <summary>
/// Repository interface for the TriageEpisode aggregate root.
/// Inherits basic CRUD operations from IBaseRepository and defines episode-specific lookups.
/// </summary>
public interface ITriageEpisodeRepository : IBaseRepository<TriageEpisode>
{
    /// <summary>
    /// Retrieves every episode of a patient, most recent first.
    /// </summary>
    /// <param name="patientId">Identifier of the patient.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The episodes of the patient.</returns>
    Task<IEnumerable<TriageEpisode>> FindByPatientIdAsync(int patientId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reports whether the patient already has an episode in progress (Waiting or InTriage).
    /// </summary>
    /// <param name="patientId">Identifier of the patient.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>True when an open episode exists.</returns>
    Task<bool> ExistsOpenByPatientIdAsync(int patientId, CancellationToken cancellationToken = default);
}
