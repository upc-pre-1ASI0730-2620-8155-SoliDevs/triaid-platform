using TriAidPlatform.API.Alerting.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.Alerting.Domain.Model.Queries;
/// <summary>
/// Query object to request all currently active medical alerts for a specific clinic.
/// </summary>
/// <param name="ClinicId">The unique identifier of the clinic to ensure data isolation.</param>
public record GetActiveAlertsQuery(ClinicId ClinicId);   