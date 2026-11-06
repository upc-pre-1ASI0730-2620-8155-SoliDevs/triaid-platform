using TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Aggregates;

namespace TriAidPlatform.API.SpecialtyAssignment.Interfaces.REST.Resources;

/// <summary>REST representation of a referral.</summary>
/// <param name="Id">Identifier of the referral.</param>
/// <param name="EpisodeId">Identifier of the episode.</param>
/// <param name="SpecialtyId">Identifier of the destination specialty.</param>
/// <param name="Room">Consulting room assigned.</param>
/// <param name="QueuePosition">Position of the patient in the queue.</param>
/// <param name="State">Lifecycle state (waiting/attended).</param>
/// <param name="ChangeReason">Justification of a manual reassignment.</param>
/// <param name="ReferredAt">UTC timestamp of the referral.</param>
public record ReferralResource(int Id, int EpisodeId, int SpecialtyId, string Room, int QueuePosition,
    string State, string? ChangeReason, DateTime ReferredAt)
{
    /// <summary>Maps the aggregate to its REST representation.</summary>
    public static ReferralResource FromEntity(Referral referral) => new(
        referral.Id, referral.EpisodeId, referral.SpecialtyId, referral.Room,
        referral.QueuePosition, referral.State, referral.ChangeReason, referral.ReferredAt);
}
