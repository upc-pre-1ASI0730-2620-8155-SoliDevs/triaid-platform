using TriAidPlatform.API.Incidents.Domain.Model.Commands;
using TriAidPlatform.API.Incidents.Domain.Model.Events;
using TriAidPlatform.API.Incidents.Domain.Model.ValueObjects;
using TriAidPlatform.API.Shared.Domain.Model.Aggregates.Internal;

namespace TriAidPlatform.API.Incidents.Domain.Model.Aggregates;

/// <summary>
/// Aggregate root that represents a clinical incident raised after detecting an abnormal reading or a priority triage condition.
/// </summary>
/// <remarks>
/// The incident encapsulates the clinical context, metric, lifecycle state, acknowledgment and escalation timestamps,
/// and the patient follow-up information associated with the incident.
/// </remarks>
public partial class Incident : AggregateRoot
{
    /// <summary>
    /// Gets the unique identifier assigned to the incident.
    /// </summary>
    public IncidentId Id { get;}

    /// <summary>
    /// Gets the clinic identifier that owns the incident and ensures logical isolation between healthcare facilities.
    /// </summary>
    public ClinicId ClinicId { get; private set; }

    /// <summary>
    /// Gets the metric that triggered the incident, such as heart rate, oxygen saturation, or blood pressure.
    /// </summary>
    public IncidentMetric Metric { get; private set; }

    /// <summary>
    /// Gets the identifier of the specific vital sign reading that caused the incident.
    /// </summary>
    public int ReadingId { get; private set; }

    /// <summary>
    /// Gets the identifier of the clinical episode associated with the incident.
    /// </summary>
    public int EpisodeId { get; private set; }

    /// <summary>
    /// Gets the incident severity level assigned by the triage or monitoring rules.
    /// </summary>
    public EIncidentLevel Level { get; private set; }

    /// <summary>
    /// Gets the current state in the incident lifecycle.
    /// </summary>
    public IncidentStatus Status { get; private set; }

    /// <summary>
    /// Gets the exact UTC timestamp when the incident was raised.
    /// </summary>
    public DateTime RaisedAt { get; private set; }

    /// <summary>
    /// Gets the destination unit or department where the incident was escalated, if applicable.
    /// </summary>
    public string? EscalatedTo { get; private set; }

    /// <summary>
    /// Gets the triage priority associated with the incident, when one is provided.
    /// </summary>
    public string? Priority { get; private set; }

    /// <summary>
    /// Gets the healthcare institution related to the patient or clinical episode.
    /// </summary>
    public string Institution { get; private set; }

    /// <summary>
    /// Gets the name of the patient associated with the incident, if available.
    /// </summary>
    public string? PatientName { get; private set; }

    /// <summary>
    /// Gets the patient's document identifier, if available.
    /// </summary>
    public string? Dni { get; private set; }

    /// <summary>
    /// Gets the exact UTC timestamp when a staff member acknowledged the incident, if it has been acknowledged.
    /// </summary>
    public DateTime? AcknowledgedAt { get; private set; }

    /// <summary>
    /// Gets the exact UTC timestamp when the incident was escalated, if it has been escalated.
    /// </summary>
    public DateTime? EscalatedAt { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Incident"/> class for ORM materialization.
    /// This constructor is protected because the aggregate is hydrated by the persistence layer.
    /// </summary>
    protected Incident()
    {
        ClinicId = new ClinicId(0);
        Institution = string.Empty;
        Metric = new IncidentMetric(string.Empty, string.Empty, string.Empty);
        Status = new IncidentStatus(EIncidentStatus.Active);
    }

    /// <summary>
    /// Initializes a new incident based on a single anomalous vital sign reading.
    /// </summary>
    /// <param name="clinicId">The clinic that owns the incident.</param>
    /// <param name="readingId">The identifier of the reading that triggered the incident.</param>
    /// <param name="level">The severity assigned to the incident.</param>
    /// <param name="metric">The vital sign metric that exceeded the incident threshold.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clinicId"/> or <paramref name="metric"/> is null.</exception>
    public Incident(ClinicId clinicId, int readingId, EIncidentLevel level, IncidentMetric metric)
    {
        ClinicId = clinicId ?? throw new ArgumentNullException(nameof(clinicId));
        Metric = metric ?? throw new ArgumentNullException(nameof(metric));
        ReadingId = readingId;
        Level = level;
        Status = new IncidentStatus(EIncidentStatus.Active);
        RaisedAt = DateTime.UtcNow;
        Institution = "Default Institution";
    }

    /// <summary>
    /// Initializes a new incident associated with a clinical episode and patient context.
    /// </summary>
    /// <param name="clinicId">The clinic responsible for the episode.</param>
    /// <param name="episodeId">The identifier of the associated clinical episode.</param>
    /// <param name="institution">The healthcare institution linked to the visit.</param>
    /// <param name="patientName">The patient's name, if available.</param>
    /// <param name="dni">The patient's identification number, if available.</param>
    /// <param name="priority">The triage or urgency priority assigned to the incident.</param>
    /// <param name="level">The incident severity category.</param>
    /// <param name="metric">The metric that caused the incident.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clinicId"/> or <paramref name="metric"/> is null.</exception>
    public Incident(ClinicId clinicId, int episodeId, string institution, string? patientName, string? dni, string? priority, EIncidentLevel level, IncidentMetric metric)
    {
        ClinicId = clinicId ?? throw new ArgumentNullException(nameof(clinicId));
        Metric = metric ?? throw new ArgumentNullException(nameof(metric));

        ReadingId = 0;
        EpisodeId = episodeId;
        Institution = institution;
        PatientName = patientName;
        Dni = dni;
        Priority = priority;
        Level = level;
        Status = new IncidentStatus(EIncidentStatus.Active);
        RaisedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Initializes an incident from a <see cref="CreateIncidentCommand"/>.
    /// </summary>
    /// <param name="command">The command containing the clinic, episode, patient and metric information.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="command"/> is null.</exception>
    public Incident(CreateIncidentCommand command) : this(
        new ClinicId((command ?? throw new ArgumentNullException(nameof(command))).ClinicId), 
        command.EpisodeId, 
        command.Institution, 
        command.PatientName, 
        command.Dni,
        command.Priority, 
        command.Level, 
        command.Metric)
    {
        AddDomainEvent(new IncidentCreatedEvent(Id,EpisodeId,Level));
    }

    /// <summary>
    /// Marks the incident as acknowledged by a healthcare professional.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the incident is already in a final state or has already been escalated.</exception>
    public void Acknowledge()
    {
        if (Status.isFinalState())
            throw new InvalidOperationException("Cannot acknowledge an already resolved incident.");

        if (Status.StatusValue == EIncidentStatus.Escalated)
            throw new InvalidOperationException("Cannot acknowledge an incident that has already been escalated.");

        Status = new IncidentStatus(EIncidentStatus.Acknowledged);
        AcknowledgedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Escalates the incident to a specific clinical destination area.
    /// </summary>
    /// <param name="destination">The area or department to which the incident should be escalated. The default value is "Trauma Shock".</param>
    /// <exception cref="ArgumentException">Thrown when the destination is empty or whitespace.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the incident is already in a final state.</exception>
    public void Escalate(string destination = "Trauma Shock")
    {
        if (string.IsNullOrWhiteSpace(destination))
            throw new ArgumentException("Must provide a valid destination for escalation.", nameof(destination));

        if (Status.isFinalState())
            throw new InvalidOperationException("Cannot escalate an incident that is already resolved.");

        Status = new IncidentStatus(EIncidentStatus.Escalated);
        EscalatedTo = destination;
        EscalatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the incident as resolved after the patient condition has stabilized or the clinical episode has concluded.
    /// </summary>
    /// <remarks>
    /// Resolution is the terminal state of the incident lifecycle. Calling this method is idempotent from the state perspective,
    /// although it does not set a separate resolution timestamp.
    /// </remarks>
    public void Resolve()
    {
        Status = new IncidentStatus(EIncidentStatus.Resolved);
    }
}