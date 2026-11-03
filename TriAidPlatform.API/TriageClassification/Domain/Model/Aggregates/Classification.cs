using TriAidPlatform.API.Shared.Domain.Model;
using TriAidPlatform.API.TriageClassification.Domain.Model.Commands;
using TriAidPlatform.API.TriageClassification.Domain.Model.Events;
using TriAidPlatform.API.TriageClassification.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.TriageClassification.Domain.Model.Aggregates;

/// <summary>
/// Aggregate root of the Triage Classification bounded context.
/// Records the NT-158 suggested level for an episode, its confirmation by
/// the nurse and any manual override with clinical justification
/// (US19-US23, US38).
/// </summary>
public class Classification : IAuditableEntity
{
    /// <summary>Unique identifier of the classification.</summary>
    public int Id { get; private set; }

    /// <summary>Identifier of the classified triage episode.</summary>
    public int EpisodeId { get; private set; }

    /// <summary>Level suggested by the NT-158 engine.</summary>
    public ETriageLevel SuggestedLevel { get; private set; }

    /// <summary>Level in force: the confirmed or manually modified one.</summary>
    public ETriageLevel FinalLevel { get; private set; }

    /// <summary>Lifecycle state of the classification.</summary>
    public EClassificationState State { get; private set; }

    /// <summary>UTC timestamp when the suggestion was computed.</summary>
    public DateTime SuggestedAt { get; private set; }

    /// <summary>UTC timestamp when the nurse confirmed the level.</summary>
    public DateTime? ConfirmedAt { get; private set; }

    /// <summary>Clinical justification required for manual changes (US22).</summary>
    public string? Justification { get; private set; }

    /// <summary>Metrics reported as missing when the suggestion was partial.</summary>
    public string? MissingMetrics { get; private set; }

    /// <summary>Creation timestamp (auditable).</summary>
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>Last update timestamp (auditable).</summary>
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>Domain events raised by the aggregate since its creation.</summary>
    public List<object> DomainEvents { get; } = [];

    /// <summary>Protected parameterless constructor required for object materialization.</summary>
    protected Classification() { }

    /// <summary>
    /// Records the NT-158 suggestion for an episode (US19). When mandatory
    /// metrics are missing the state stays Suggested with the missing list.
    /// </summary>
    public Classification(SuggestClassificationCommand command)
    {
        EpisodeId = command.EpisodeId;
        SuggestedLevel = command.SuggestedLevel;
        FinalLevel = command.SuggestedLevel;
        State = EClassificationState.Suggested;
        SuggestedAt = DateTime.UtcNow;
        MissingMetrics = command.MissingMetrics.Length > 0 ? string.Join(",", command.MissingMetrics) : null;
    }

    /// <summary>Whether the level is already decided by the nurse.</summary>
    public bool IsDecided() => State is EClassificationState.Confirmed or EClassificationState.Modified;

    /// <summary>
    /// Confirms the suggested level as the final one (US20 - Escenario 1).
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the classification is already decided.</exception>
    public void Confirm()
    {
        if (IsDecided())
            throw new InvalidOperationException("The classification already has a final level.");
        State = EClassificationState.Confirmed;
        ConfirmedAt = DateTime.UtcNow;
        DomainEvents.Add(new ClassificationConfirmedEvent(Id, EpisodeId, FinalLevel));
    }

    /// <summary>
    /// Replaces the level with a manual override that requires a written
    /// justification (US21 - Escenario 1, US22 - Escenario 1).
    /// </summary>
    /// <param name="newLevel">Clinically justified level.</param>
    /// <param name="justification">Mandatory written reason.</param>
    /// <exception cref="InvalidOperationException">Thrown without justification or when already decided.</exception>
    public void Modify(ETriageLevel newLevel, string justification)
    {
        if (IsDecided())
            throw new InvalidOperationException("The classification already has a final level.");
        if (string.IsNullOrWhiteSpace(justification))
            throw new InvalidOperationException("A written justification is required to change the suggested level.");
        FinalLevel = newLevel;
        State = EClassificationState.Modified;
        Justification = justification.Trim();
        ConfirmedAt = DateTime.UtcNow;
        DomainEvents.Add(new ClassificationModifiedEvent(Id, EpisodeId, newLevel, Justification));
    }
}
