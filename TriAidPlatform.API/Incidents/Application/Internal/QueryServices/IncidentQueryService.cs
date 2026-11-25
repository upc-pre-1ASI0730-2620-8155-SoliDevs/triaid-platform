using TriAidPlatform.API.Incidents.Domain.Model.Aggregates;
using TriAidPlatform.API.Incidents.Domain.Model.Queries;
using TriAidPlatform.API.Incidents.Domain.Repositories;

namespace TriAidPlatform.API.Incidents.Application.Internal.QueryServices;

/// <summary>
/// Service responsible for handling read-only queries related to the Alert aggregate.
/// Utilizes a primary constructor to inject the required data access dependencies.
/// </summary>
/// <param name="incidentRepository">The repository interface used to query alert data from the database.</param>
public class IncidentQueryService(IIncidentRepository incidentRepository)
{
    /// <summary>
    /// Handles the execution of the <see cref="GetActiveIncidentsQuery"/> to retrieve active alerts.
    /// Enforces multi-tenant data isolation by filtering alerts strictly based on the provided ClinicId.
    /// </summary>
    /// <param name="query">The query object containing the ClinicId constraint.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>An asynchronous task returning a collection of active <see cref="Incident"/> instances for the specified clinic.</returns>
    public async Task<IEnumerable<Incident>> Handle(GetActiveIncidentsQuery query, CancellationToken cancellationToken = default)
    {
        
        return await incidentRepository.FindActiveAlertsAsync(query.ClinicId, cancellationToken);
    }
    /// <summary>
    /// Handles the execution of the <see cref="ListAllIncidentsQuery"/> to retrieve all alerts globally.
    /// </summary>
    public async Task<IEnumerable<Incident>> Handle(ListAllIncidentsQuery query)
    {
        return await incidentRepository.ListAsync();
    }

    /// <summary>
    /// Handles the execution of the <see cref="GetIncidentByIdQuery"/> to retrieve a specific alert.
    /// </summary>
    public async Task<Incident?> Handle(GetIncidentByIdQuery query)
    {
        return await incidentRepository.GetByIdAsync(query.AlertId);
    }
}