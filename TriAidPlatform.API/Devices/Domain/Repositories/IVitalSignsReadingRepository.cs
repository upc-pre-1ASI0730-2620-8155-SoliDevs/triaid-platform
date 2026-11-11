using TriAidPlatform.API.Devices.Domain.Model.Aggregates;

namespace TriAidPlatform.API.Devices.Domain.Repositories;

/// <summary>
/// Repository interface for <see cref="VitalSignsReading"/> aggregate root operations.
/// </summary>
public interface IVitalSignsReadingRepository
{
    /// <summary>Finds a biometric reading by its unique identifier.</summary>
    Task<VitalSignsReading?> FindByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>Retrieves all readings associated with a specific triage episode.</summary>
    Task<IEnumerable<VitalSignsReading>> ListByEpisodeIdAsync(int episodeId, CancellationToken cancellationToken);

    /// <summary>Retrieves all biometric readings.</summary>
    Task<IEnumerable<VitalSignsReading>> ListAsync(CancellationToken cancellationToken);

    /// <summary>Adds a new biometric reading to the persistence store.</summary>
    Task AddAsync(VitalSignsReading reading, CancellationToken cancellationToken);
}