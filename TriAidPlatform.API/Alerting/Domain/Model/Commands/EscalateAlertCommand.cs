using TriAidPlatform.API.Alerting.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.Alerting.Domain.Model.Commands;

/// <summary>
/// Command to escalate a critical medical alert, routing the patient to a specific specialized unit.
/// </summary>
/// <param name="ClinicId">The unique tenant identifier to ensure data isolation.</param>
/// <param name="AlertId">The unique identifier of the alert being escalated.</param>
/// <param name="Destination">The target unit for the escalation (e.g., "Trauma Shock").</param>
public record EscalateAlertCommand(ClinicId ClinicId, int AlertId, string Destination);
