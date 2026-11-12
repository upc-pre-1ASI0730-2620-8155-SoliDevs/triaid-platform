namespace TriAidPlatform.API.Alerting.Interfaces.Rest.Resources;

/**
 * <summary>
 *     Alert body used by the web client triage flow, tolerant of a null
 *     identifier on creation (ADR-010).
 * </summary>
 * <param name="Episode">Identifier of the episode as reported by the client.</param>
 * <param name="Institution">Institution the alert belongs to.</param>
 * <param name="PatientName">Patient display name.</param>
 * <param name="Dni">Patient national identifier.</param>
 * <param name="Priority">Triage priority code (I to V).</param>
 * <param name="VitalSign">Label of the altered vital sign or triage check.</param>
 * <param name="Value">Measured value with its unit.</param>
 * <param name="SafeRange">Reference range of the vital sign.</param>
 * <param name="Severity">Severity key (critical or warning).</param>
 * <param name="CreatedAt">Client-reported creation timestamp.</param>
 */
public record CreateAlertResource(
    string? Episode,
    string Institution,
    string? PatientName,
    string? Dni,
    string? Priority,
    string VitalSign,
    string Value,
    string SafeRange,
    string Severity,
    DateTime? CreatedAt);

/**
 * <summary>
 *     Alert body used by the web client acknowledge and escalate flows
 *     (ADR-010).
 * </summary>
 * <param name="Severity">Target severity (resolved or escalated).</param>
 * <param name="EscalatedTo">Destination of the escalation, when escalating.</param>
 * <param name="Institution">Institution the alert belongs to.</param>
 */
public record UpdateAlertResource(string Severity, string? EscalatedTo, string? Institution);
