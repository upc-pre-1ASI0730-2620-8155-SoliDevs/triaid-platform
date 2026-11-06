namespace TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Events;

/// <summary>Domain event raised after a justified reassignment (US32).</summary>
/// <param name="ReferralId">Identifier of the referral.</param>
/// <param name="EpisodeId">Identifier of the episode.</param>
/// <param name="NewSpecialtyId">Identifier of the new specialty.</param>
/// <param name="Room">Consulting room of the new specialty.</param>
/// <param name="Reason">Written justification of the change.</param>
public record ReferralReassignedEvent(int ReferralId, int EpisodeId, int NewSpecialtyId, string Room, string Reason);
