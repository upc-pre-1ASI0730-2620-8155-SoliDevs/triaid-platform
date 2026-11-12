using TriAidPlatform.API.TriageClassification.Domain.Model.Aggregates;
using TriAidPlatform.API.Shared.Domain.Repositories;

namespace TriAidPlatform.API.TriageClassification.Domain.Repositories;

/// <summary>
/// Repository interface for the Classification aggregate root. Inherits the
/// basic CRUD contract and defines the episode and tenant lookups.
/// </summary>
public interface IClassificationRepository : IBaseRepository<Classification>
{
    /// <summary>Retrieves the classification of an episode.</summary>
    /// <param name="episodeId">Identifier of the episode.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>The classification, or null when the episode is not classified yet.</returns>
    Task<Classification?> FindByEpisodeIdAsync(string episodeId, CancellationToken cancellationToken = default);

    /// <summary>Lists the classifications of a clinic, most recent first.</summary>
    /// <param name="clinicId">Identifier of the clinic (tenant).</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>The classifications of the clinic.</returns>
    Task<IEnumerable<Classification>> FindByClinicIdAsync(int clinicId, CancellationToken cancellationToken = default);
}
