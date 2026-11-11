namespace TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Events;

/// <summary>Domain event raised when an episode is referred (US31).</summary>
/// <param name="ReferralId">Identifier of the referral.</param>
/// <param name="EpisodeId">Identifier of the episode.</param>
/// <param name="SpecialtyId">Identifier of the destination specialty.</param>
/// <param name="Room">Consulting room assigned.</param>
/// <param name="QueuePosition">Position of the patient in the queue.</param>
public record ReferralCreatedEvent(int ReferralId, string EpisodeId, int SpecialtyId, string Room, int QueuePosition);
