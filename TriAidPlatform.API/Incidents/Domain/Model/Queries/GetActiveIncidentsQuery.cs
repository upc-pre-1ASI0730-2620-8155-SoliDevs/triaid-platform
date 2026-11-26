using TriAidPlatform.API.Incidents.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.Incidents.Domain.Model.Queries;
/// <summary>
/// Query object to request all currently active incidents for a specific clinic.
/// </summary>
/// <param name="ClinicId">The unique identifier of the clinic to ensure data isolation.</param>
public record GetActiveIncidentsQuery(ClinicId ClinicId);   