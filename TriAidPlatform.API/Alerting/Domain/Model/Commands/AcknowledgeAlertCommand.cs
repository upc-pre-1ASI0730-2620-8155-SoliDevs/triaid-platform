using TriAidPlatform.API.Alerting.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.Alerting.Domain.Model.Commands;

/// <summary>
/// Command to acknowledge an active medical alert.
/// </summary>
/// <param name="ClinicId">The unique tenant identifier (Value Object) to ensure data isolation.</param>
/// <param name="AlertId">The unique identifier of the alert to be acknowledged.</param>
public record AcknowledgeAlertCommand(ClinicId ClinicId, int AlertId);