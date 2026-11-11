using TriAidPlatform.API.TriageClassification.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.TriageClassification.Interfaces.REST.Resources;

/// <summary>REST representation of a triage classification.</summary>
/// <param name="Id">Identifier of the classification.</param>
/// <param name="EpisodeId">Identifier of the classified episode.</param>
/// <param name="SuggestedLevel">Level suggested by the NT-158 engine.</param>
/// <param name="FinalLevel">Level in force.</param>
/// <param name="State">Lifecycle state (suggested/confirmed/modified).</param>
/// <param name="SuggestedAt">UTC timestamp of the suggestion.</param>
/// <param name="ConfirmedAt">UTC timestamp of the confirmation.</param>
/// <param name="Justification">Written reason of a manual override.</param>
/// <param name="MissingMetrics">Metrics reported missing during the evaluation.</param>
public record ClassificationResource(
    int Id,
    string EpisodeId,
    ETriageLevel SuggestedLevel,
    ETriageLevel FinalLevel,
    EClassificationState State,
    DateTime SuggestedAt,
    DateTime? ConfirmedAt,
    string? Justification,
    string? MissingMetrics);
