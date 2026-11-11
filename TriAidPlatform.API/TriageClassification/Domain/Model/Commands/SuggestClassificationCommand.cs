using TriAidPlatform.API.TriageClassification.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.TriageClassification.Domain.Model.Commands;

/// <summary>
/// Command that records the NT-158 suggestion for a triage episode (US19).
/// </summary>
/// <param name="EpisodeId">Identifier of the classified episode.</param>
/// <param name="SuggestedLevel">Level computed by the NT-158 engine.</param>
/// <param name="MissingMetrics">Mandatory metrics that were not available.</param>
public record SuggestClassificationCommand(string EpisodeId, ETriageLevel SuggestedLevel, string[] MissingMetrics);
