namespace TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Queries;

/// <summary>Query that retrieves the referral of an episode.</summary>
/// <param name="EpisodeId">Identifier of the episode.</param>
public record GetReferralByEpisodeQuery(string EpisodeId);
