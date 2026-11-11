using TriAidPlatform.API.Shared.Domain.Model;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Commands;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Events;

namespace TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Aggregates;

/// <summary>
/// Aggregate root that records the routing of a classified episode to a
/// specialty and its consulting room, with the live queue position
/// (US30-US33, US37) and the justified manual reassignment (US32).
/// </summary>
public class Referral : IAuditableEntity
{
    /// <summary>Unique identifier of the referral.</summary>
    public int Id { get; private set; }

    /// <summary>Identifier of the referred triage episode.</summary>
    public string EpisodeId { get; private set; } = string.Empty;

    /// <summary>Identifier of the destination specialty.</summary>
    public int SpecialtyId { get; private set; }

    /// <summary>Consulting room of the destination specialty at referral time.</summary>
    public string Room { get; private set; } = string.Empty;

    /// <summary>Live position of the patient in the specialty queue (US37).</summary>
    public int QueuePosition { get; private set; }

    /// <summary>Lifecycle state of the referral (waiting/attended).</summary>
    public string State { get; private set; } = "waiting";

    /// <summary>Written justification of a manual reassignment (US32).</summary>
    public string? ChangeReason { get; private set; }

    /// <summary>UTC timestamp when the patient was referred.</summary>
    public DateTime ReferredAt { get; private set; }

    /// <summary>Main symptom registered with the referral (web client contract).</summary>
    public string? Symptom { get; private set; }

    /// <summary>Institution the referral belongs to (web client tenancy).</summary>
    public string Institution { get; private set; } = string.Empty;

    /// <summary>Creation timestamp (auditable).</summary>
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>Last update timestamp (auditable).</summary>
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>Domain events raised by the aggregate since its creation.</summary>
    public List<object> DomainEvents { get; } = [];

    /// <summary>Protected parameterless constructor required for object materialization.</summary>
    protected Referral() { }

    /// <summary>
    /// Creates the referral of an episode to a specialty with its queue
    /// position (US31 - Escenario 1).
    /// </summary>
    public Referral(ReferEpisodeCommand command, string room, int queuePosition)
    {
        EpisodeId = command.EpisodeId;
        SpecialtyId = command.SpecialtyId;
        Room = room;
        QueuePosition = queuePosition;
        ReferredAt = DateTime.UtcNow;
        AddDomainEvent(new ReferralCreatedEvent(Id, EpisodeId, SpecialtyId, Room, QueuePosition));
    }

    private void AddDomainEvent(object domainEvent) => DomainEvents.Add(domainEvent);

    /// <summary>
    /// Reassigns the referral to another specialty with a mandatory
    /// justification (US32 - Escenario 1).
    /// </summary>
    /// <param name="newSpecialtyId">Identifier of the new specialty.</param>
    /// <param name="room">Consulting room of the new specialty.</param>
    /// <param name="queuePosition">Position of the patient in the new queue.</param>
    /// <param name="reason">Mandatory written justification.</param>
    /// <exception cref="InvalidOperationException">Thrown without justification.</exception>
    public void Reassign(int newSpecialtyId, string room, int queuePosition, string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new InvalidOperationException("A written justification is required to reassign a referral.");
        if (newSpecialtyId == SpecialtyId) return;
        SpecialtyId = newSpecialtyId;
        Room = room;
        QueuePosition = queuePosition;
        ChangeReason = reason.Trim();
        DomainEvents.Add(new ReferralReassignedEvent(Id, EpisodeId, newSpecialtyId, Room, reason.Trim()));
    }

    /// <summary>Marks the referral as attended in the specialty (US32 - Escenario 3).</summary>
    public void MarkAttended() => State = "attended";

    /// <summary>Whether the patient is still waiting in the queue.</summary>
    public bool IsWaiting() => State == "waiting";

    /**
     * <summary>
     *     Attach the web client context of the referral: main symptom,
     *     owning institution and the client-reported timestamps (ADR-010)
     * </summary>
     * <param name="symptom">Main symptom text.</param>
     * <param name="institution">Institution the referral belongs to.</param>
     * <param name="state">Lifecycle state reported by the client.</param>
     * <param name="referredAt">Client-reported referral timestamp.</param>
     * <returns>The updated referral.</returns>
     */
    public Referral AttachContext(string? symptom, string institution, string? state, DateTime? referredAt)
    {
        Symptom = symptom;
        Institution = institution;
        if (!string.IsNullOrWhiteSpace(state)) State = state;
        if (referredAt.HasValue) ReferredAt = referredAt.Value;
        return this;
    }

    /**
     * <summary>
     *     Apply a full-record update from the web client: specialty,
     *     room, queue position, state and justification (ADR-010)
     * </summary>
     * <param name="specialtyId">Identifier of the destination specialty.</param>
     * <param name="room">Consulting room of the specialty.</param>
     * <param name="queuePosition">Position of the patient in the queue.</param>
     * <param name="state">Lifecycle state reported by the client.</param>
     * <param name="changeReason">Written justification when reassigned.</param>
     * <returns>The updated referral.</returns>
     */
    public Referral ApplyRecordUpdate(int specialtyId, string room, int queuePosition,
        string? state, string? changeReason)
    {
        if (specialtyId != SpecialtyId)
        {
            SpecialtyId = specialtyId;
            Room = room;
            QueuePosition = queuePosition;
            ChangeReason = string.IsNullOrWhiteSpace(changeReason) ? ChangeReason : changeReason.Trim();
        }
        else
        {
            Room = string.IsNullOrWhiteSpace(room) ? Room : room;
            QueuePosition = queuePosition;
            ChangeReason = string.IsNullOrWhiteSpace(changeReason) ? ChangeReason : changeReason.Trim();
        }
        if (!string.IsNullOrWhiteSpace(state)) State = state;
        return this;
    }
}
