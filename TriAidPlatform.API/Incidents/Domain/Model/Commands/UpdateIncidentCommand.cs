namespace TriAidPlatform.API.Incidents.Domain.Model.Commands;

/// <summary>
/// Contains the values used to update an existing incident.
/// </summary>
/// <remarks>
/// The severity and optional escalation destination are applied to the identified incident
/// by the incident command service.
/// </remarks>
/// <param name="IncidentId">The identifier of the incident to update.</param>
/// <param name="Severity">The severity value to assign to the incident.</param>
/// <param name="EscalatedTo">The escalation destination to assign, if applicable.</param>
public record UpdateIncidentCommand(
    int IncidentId,
    string Severity,
    string? EscalatedTo
);