using TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Aggregates;
using TriAidPlatform.API.Shared.Domain.Repositories;

namespace TriAidPlatform.API.SpecialtyAssignment.Domain.Repositories;

/// <summary>Repository interface of the Referral aggregate root.</summary>
public interface IReferralRepository : IBaseRepository<Referral>
{
    /// <summary>Retrieves the referral of an episode.</summary>
    /// <param name="episodeId">Identifier of the episode.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>The referral, or null when the episode is not referred yet.</returns>
    Task<Referral?> FindByEpisodeIdAsync(string episodeId, CancellationToken cancellationToken = default);

    /// <summary>Counts the patients waiting in the queue of a specialty (US37).</summary>
    /// <param name="specialtyId">Identifier of the specialty.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>Number of waiting referrals ahead of a new patient.</returns>
    Task<int> CountWaitingBySpecialtyAsync(int specialtyId, CancellationToken cancellationToken = default);
}
