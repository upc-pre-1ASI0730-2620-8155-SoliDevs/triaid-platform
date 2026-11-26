using TriAidPlatform.API.Incidents.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.Incidents.Domain.Model.Commands;

/// <summary>
/// Contains the information required to escalate an incident to a clinical destination.
/// </summary>
/// <remarks>
/// The clinic identifier is used to enforce tenant isolation when the incident is retrieved.
/// </remarks>
/// <param name="ClinicId">The identifier of the clinic that owns the incident.</param>
/// <param name="IncidentId">The identifier of the incident to escalate.</param>
/// <param name="Destination">The target unit or department for the escalation.</param>
public record EscalateIncidentCommand(ClinicId ClinicId, int IncidentId, string Destination);
