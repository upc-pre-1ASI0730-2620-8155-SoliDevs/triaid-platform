namespace TriAidPlatform.API.TriageClassification.Interfaces.REST.Resources;

/// <summary>REST representation of a triage classification shaped for the web client.</summary>
/// <param name="Id">Identifier of the classification.</param>
/// <param name="EpisodeId">Identifier of the classified episode.</param>
/// <param name="SuggestedLevel">Level suggested by the NT-158 engine.</param>
/// <param name="Level">Level in force.</param>
/// <param name="State">Lifecycle state (PendingConfirmation/Assigned/Overridden/Confirmed).</param>
/// <param name="SuggestedAt">UTC timestamp of the suggestion.</param>
/// <param name="ConfirmedAt">UTC timestamp of the confirmation.</param>
/// <param name="Justification">Written reason of a manual override.</param>
/// <param name="MissingMetrics">Metrics reported missing during the evaluation.</param>
/// <param name="OverriddenBy">Staff member who overrode the suggested level.</param>
/// <param name="Institution">Institution the classification belongs to.</param>
public record ClassificationResource(
    int Id,
    string EpisodeId,
    string SuggestedLevel,
    string Level,
    string State,
    DateTime SuggestedAt,
    DateTime? ConfirmedAt,
    string? Justification,
    string? MissingMetrics,
    string? OverriddenBy,
    string Institution);
