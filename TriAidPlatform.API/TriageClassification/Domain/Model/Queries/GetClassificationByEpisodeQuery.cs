namespace TriAidPlatform.API.TriageClassification.Domain.Model.Queries;

/// <summary>Query that retrieves the classification of an episode.</summary>
/// <param name="EpisodeId">Identifier of the episode.</param>
public record GetClassificationByEpisodeQuery(string EpisodeId);
