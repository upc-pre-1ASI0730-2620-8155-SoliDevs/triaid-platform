namespace TriAidPlatform.API.Alerting.Application.Internal.QueryServices;
/// <summary>
/// Query object to request all currently active medical alerts for a specific clinic.
/// </summary>
/// <param name="ClinicId">The unique identifier of the clinic to ensure data isolation.</param>
public record GetActiveAlertsQuery(int ClinicId);