namespace TriAidPlatform.API.Alerting.Domain.Model.Commands;

/// <summary>
/// Command containing the values used to update an existing alert's severity and escalation destination.
/// </summary>
/// <param name="AlertId">The identifier of the alert to update.</param>
/// <param name="Severity">The severity value to assign to the alert.</param>
/// <param name="EscalatedTo">The escalation destination to assign, if applicable.</param>
public record UpdateAlertCommand(
    int AlertId,
    string Severity,
    string? EscalatedTo
);