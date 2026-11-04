using TriAidPlatform.API.PatientRegistration.Domain.Model.Commands;
using TriAidPlatform.API.PatientRegistration.Domain.Model.ValueObjects;
using TriAidPlatform.API.Shared.Domain.Model;

namespace TriAidPlatform.API.PatientRegistration.Domain.Model.Aggregates;

/// <summary>
/// Aggregate root that represents one emergency visit of a patient.
/// Its identifier is the episode_id that the other bounded contexts use to keep
/// the traceability of the attention.
/// </summary>
public class TriageEpisode : IAuditableEntity
{
    /// <summary>Unique identifier of the episode.</summary>
    public int Id { get; private set; }

    /// <summary>Identifier of the patient that owns the episode.</summary>
    public int PatientId { get; private set; }

    /// <summary>UTC date and time when the patient arrived at the emergency service.</summary>
    public DateTime ArrivalTime { get; private set; }

    /// <summary>Current lifecycle state of the episode.</summary>
    public EEpisodeState State { get; private set; }

    /// <summary>Creation timestamp (auditable).</summary>
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>Last update timestamp (auditable).</summary>
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Protected parameterless constructor strictly required for object materialization.
    /// </summary>
    protected TriageEpisode() { }

    /// <summary>
    /// Opens a new episode in the Waiting state with the current arrival time.
    /// </summary>
    /// <param name="command">The patient that arrives.</param>
    /// <exception cref="ArgumentException">Thrown when the patient identifier is not valid.</exception>
    public TriageEpisode(OpenTriageEpisodeCommand command)
    {
        if (command.PatientId <= 0)
            throw new ArgumentException("A valid patient is required to open a triage episode.");

        PatientId = command.PatientId;
        ArrivalTime = DateTime.UtcNow;
        State = EEpisodeState.Waiting;
    }

    /// <summary>Whether the episode is still in progress.</summary>
    public bool IsOpen() => State is EEpisodeState.Waiting or EEpisodeState.InTriage;

    /// <summary>
    /// Moves the episode from Waiting to InTriage when the evaluation starts.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the episode is not waiting.</exception>
    public void StartTriage()
    {
        if (State != EEpisodeState.Waiting)
            throw new InvalidOperationException("Only a waiting episode can start triage.");
        State = EEpisodeState.InTriage;
    }

    /// <summary>
    /// Closes the episode as attended in the emergency service.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the episode is not in triage.</exception>
    public void MarkAttended()
    {
        if (State != EEpisodeState.InTriage)
            throw new InvalidOperationException("Only an episode in triage can be marked as attended.");
        State = EEpisodeState.Attended;
    }

    /// <summary>
    /// Closes the episode as referred to a specialty.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the episode is not in triage.</exception>
    public void MarkReferred()
    {
        if (State != EEpisodeState.InTriage)
            throw new InvalidOperationException("Only an episode in triage can be referred.");
        State = EEpisodeState.Referred;
    }
}
