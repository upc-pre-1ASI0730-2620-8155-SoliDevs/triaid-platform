using TriAidPlatform.API.Incidents.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.Incidents.Domain.Model.Commands;

/// <summary>
/// Contains the information required to acknowledge an active incident.
/// </summary>
/// <remarks>
/// The clinic identifier is used to enforce tenant isolation when the incident is retrieved.
/// </remarks>
/// <param name="ClinicId">The identifier of the clinic that owns the incident.</param>
/// <param name="IncidentId">The identifier of the incident to acknowledge.</param>
public record AcknowledgeIncidentCommand(ClinicId ClinicId, int IncidentId);