namespace TriAidPlatform.API.Alerting.Interfaces.Rest.Resources;
/// <summary>
/// Resource payload containing the details required to escalate an alert.
/// </summary>
/// <param name="Destination">The target department or care area for the escalation (e.g., "Trauma Shock").</param>

public record EscalateAlertResource(string Destination);