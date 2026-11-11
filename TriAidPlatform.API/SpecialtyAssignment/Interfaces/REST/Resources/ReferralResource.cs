namespace TriAidPlatform.API.SpecialtyAssignment.Interfaces.REST.Resources;

/// <summary>REST representation of a referral shaped for the web client.</summary>
/// <param name="Id">Identifier of the referral.</param>
/// <param name="EpisodeId">Identifier of the episode.</param>
/// <param name="Specialty">Name of the destination specialty.</param>
/// <param name="Room">Consulting room assigned.</param>
/// <param name="QueuePosition">Position of the patient in the queue.</param>
/// <param name="State">Lifecycle state (waiting/attended/Referred).</param>
/// <param name="ChangeReason">Justification of a manual reassignment.</param>
/// <param name="ReferredAt">UTC timestamp of the referral.</param>
/// <param name="Symptom">Main symptom registered with the referral.</param>
/// <param name="Institution">Institution the referral belongs to.</param>
public record ReferralResource(int Id, string EpisodeId, string Specialty, string Room, int QueuePosition,
    string State, string? ChangeReason, DateTime ReferredAt, string? Symptom, string Institution);
