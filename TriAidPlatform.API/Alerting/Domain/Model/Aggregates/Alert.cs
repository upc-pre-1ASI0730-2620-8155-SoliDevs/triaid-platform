using TriAidPlatform.API.Alerting.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.Alerting.Domain.Model.Aggregates;

/// <summary>
/// Aggregate root that represents a clinical alert raised after detecting an abnormal reading or a priority triage condition.
/// This aggregate encapsulates the alert lifecycle, escalation flow, and the minimum information needed for patient follow-up.
/// </summary>
public class Alert
{
    /// <summary>
    /// Gets the unique identifier assigned to the alert.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Gets the clinic identifier that owns the alert and ensures logical isolation between healthcare facilities.
    /// </summary>
    public ClinicId ClinicId { get; private set; }

    /// <summary>
    /// Gets the metric that triggered the alert, such as heart rate, oxygen saturation, or blood pressure.
    /// </summary>
    public AlertMetric Metric { get; private set; }

    /// <summary>
    /// Gets the identifier of the specific vital sign reading that caused the alert.
    /// </summary>
    public int ReadingId { get; private set; }

    /// <summary>
    /// Gets the identifier of the clinical episode associated with the alert.
    /// </summary>
    public int EpisodeId { get; private set; }

    /// <summary>
    /// Gets the alert severity level assigned by the triage or monitoring rules.
    /// </summary>
    public EAlertLevel Level { get; private set; }

    /// <summary>
    /// Gets the current state in the alert lifecycle.
    /// </summary>
    public AlertStatus Status { get; private set; }

    /// <summary>
    /// Gets the exact UTC timestamp when the alert was raised.
    /// </summary>
    public DateTime RaisedAt { get; private set; }

    /// <summary>
    /// Gets the destination unit or department where the alert was escalated, if applicable.
    /// </summary>
    public string? EscalatedTo { get; private set; }

    /// <summary>
    /// Gets the triage priority associated with the alert, when one is provided.
    /// </summary>
    public string? Priority { get; private set; }

    /// <summary>
    /// Gets the healthcare institution related to the patient or clinical episode.
    /// </summary>
    public string Institution { get; private set; }

    /// <summary>
    /// Gets the name of the patient associated with the alert, if available.
    /// </summary>
    public string? PatientName { get; private set; }

    /// <summary>
    /// Gets the patient's document identifier, if available.
    /// </summary>
    public string? Dni { get; private set; }

    /// <summary>
    /// Gets the exact UTC timestamp when a staff member acknowledged the alert, if it has been acknowledged.
    /// </summary>
    public DateTime? AcknowledgedAt { get; private set; }

    /// <summary>
    /// Gets the exact UTC timestamp when the alert was escalated, if it has been escalated.
    /// </summary>
    public DateTime? EscalatedAt { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Alert"/> class for ORM materialization.
    /// This constructor is protected because the aggregate is hydrated by the persistence layer.
    /// </summary>
    protected Alert() { }

    /// <summary>
    /// Initializes a new alert based on a single anomalous vital sign reading.
    /// </summary>
    /// <param name="clinicId">The clinic that owns the alert.</param>
    /// <param name="readingId">The identifier of the reading that triggered the alert.</param>
    /// <param name="level">The severity assigned to the alert.</param>
    /// <param name="metric">The vital sign metric that exceeded the alert threshold.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clinicId"/> or <paramref name="metric"/> is null.</exception>
    public Alert(ClinicId clinicId, int readingId, EAlertLevel level, AlertMetric metric)
    {
        ClinicId = clinicId ?? throw new ArgumentNullException(nameof(clinicId));
        Metric = metric ?? throw new ArgumentNullException(nameof(metric));
        ReadingId = readingId;
        Level = level;
        Status = new AlertStatus(EAlertStatus.Active);
        RaisedAt = DateTime.UtcNow;
        Institution = "Default Institution";
    }

    /// <summary>
    /// Initializes a new alert associated with a clinical episode and patient context.
    /// </summary>
    /// <param name="clinicId">The clinic responsible for the episode.</param>
    /// <param name="episodeId">The identifier of the associated clinical episode.</param>
    /// <param name="institution">The healthcare institution linked to the visit.</param>
    /// <param name="patientName">The patient's name, if available.</param>
    /// <param name="dni">The patient's identification number, if available.</param>
    /// <param name="priority">The triage or urgency priority assigned to the alert.</param>
    /// <param name="level">The alert severity category.</param>
    /// <param name="metric">The metric that caused the alert.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="clinicId"/> or <paramref name="metric"/> is null.</exception>
    public Alert(ClinicId clinicId, int episodeId, string institution, string? patientName, string? dni, string? priority, EAlertLevel level, AlertMetric metric)
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
        Status = new AlertStatus(EAlertStatus.Active);
        RaisedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the alert as acknowledged by a healthcare professional.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the alert is already in a final state or has already been escalated.</exception>
    public void Acknowledge()
    {
        if (Status.isFinalState())
            throw new InvalidOperationException("Cannot acknowledge an already resolved alert.");

        if (Status.StatusValue == EAlertStatus.Escalated)
            throw new InvalidOperationException("Cannot acknowledge an alert that has already been escalated.");

        Status = new AlertStatus(EAlertStatus.Acknowledged);
        AcknowledgedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Escalates the alert to a specific clinical destination area.
    /// </summary>
    /// <param name="destination">The area or department to which the alert should be escalated. The default value is "Trauma Shock".</param>
    /// <exception cref="ArgumentException">Thrown when the destination is empty or whitespace.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the alert is already in a final state.</exception>
    public void Escalate(string destination = "Trauma Shock")
    {
        if (string.IsNullOrWhiteSpace(destination))
            throw new ArgumentException("Must provide a valid destination for escalation.", nameof(destination));

        if (Status.isFinalState())
            throw new InvalidOperationException("Cannot escalate an alert that is already resolved.");

        Status = new AlertStatus(EAlertStatus.Escalated);
        EscalatedTo = destination;
        EscalatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the alert as resolved after the patient condition has stabilized or the episode has concluded.
    /// </summary>
    public void Resolve()
    {
        Status = new AlertStatus(EAlertStatus.Resolved);
    }
}