namespace TriAidPlatform.API.Alerting.Interfaces.Rest.Resources;
/// <summary>
/// REST response resource containing alert details and associated patient and triage information.
/// This data-transfer object exposes a flattened representation intended for API clients.
/// </summary>
/// <param name="Id">The unique identifier of the alert.</param>
/// <param name="Institution">The healthcare institution associated with the alert.</param>
/// <param name="PatientName">The patient's name, if available.</param>
/// <param name="Dni">The patient's identification document number, if available.</param>
/// <param name="Episode">The identifier of the clinical episode associated with the alert.</param>
/// <param name="Priority">The triage priority associated with the alert, if available.</param>
/// <param name="VitalSign">The name of the vital sign or metric associated with the alert.</param>
/// <param name="Value">The measured value associated with the alert.</param>
/// <param name="SafeRange">The expected or safe range for the vital sign.</param>
/// <param name="Time">The time associated with the alert's vital sign measurement, represented as text.</param>
/// <param name="Severity">The severity assigned to the alert.</param>
/// <param name="AcknowledgedAt">The acknowledgment timestamp, if the alert has been acknowledged.</param>
/// <param name="EscalatedAt">The escalation timestamp, if the alert has been escalated.</param>
/// <param name="EscalatedTo">The destination to which the alert was escalated, if applicable.</param>
/// <param name="CreatedAt">The timestamp when the alert was created, represented as text.</param>
public record AlertResource(
    int Id,
    string Institution,
    string? PatientName,
    string? Dni,
    int Episode,
    string? Priority,
    string VitalSign,
    string Value,
    string SafeRange,
    string Time,
    string Severity,
    string? AcknowledgedAt,
    string? EscalatedAt,
    string? EscalatedTo,
    string CreatedAt
);