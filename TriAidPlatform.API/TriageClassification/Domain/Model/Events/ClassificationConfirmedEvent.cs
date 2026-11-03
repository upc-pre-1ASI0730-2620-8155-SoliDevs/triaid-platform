using TriAidPlatform.API.TriageClassification.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.TriageClassification.Domain.Model.Events;

/// <summary>Domain event raised when the nurse confirms the level (US20).</summary>
/// <param name="ClassificationId">Identifier of the classification.</param>
/// <param name="EpisodeId">Identifier of the episode.</param>
/// <param name="FinalLevel">Level that rules the attention.</param>
public record ClassificationConfirmedEvent(int ClassificationId, int EpisodeId, ETriageLevel FinalLevel);
