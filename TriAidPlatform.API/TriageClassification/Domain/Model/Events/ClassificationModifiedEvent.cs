using TriAidPlatform.API.TriageClassification.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.TriageClassification.Domain.Model.Events;

/// <summary>Domain event raised after a justified manual override (US21/US22).</summary>
/// <param name="ClassificationId">Identifier of the classification.</param>
/// <param name="EpisodeId">Identifier of the episode.</param>
/// <param name="NewLevel">Clinically justified level.</param>
/// <param name="Justification">Written reason of the change.</param>
public record ClassificationModifiedEvent(int ClassificationId, int EpisodeId, ETriageLevel NewLevel, string Justification);
