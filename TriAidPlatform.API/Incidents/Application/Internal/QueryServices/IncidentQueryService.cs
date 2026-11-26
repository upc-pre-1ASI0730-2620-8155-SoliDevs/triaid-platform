using TriAidPlatform.API.Incidents.Application.QueryServices;
using TriAidPlatform.API.Incidents.Domain.Model.Aggregates;
using TriAidPlatform.API.Incidents.Domain.Model.Queries;
using TriAidPlatform.API.Incidents.Domain.Repositories;

namespace TriAidPlatform.API.Incidents.Application.Internal.QueryServices;

/// <summary>
/// Handles read-only queries for <see cref="Incident"/> aggregates.
/// </summary>
/// <param name="incidentRepository">The repository used to retrieve incident data.</param>
public class IncidentQueryService(IIncidentRepository incidentRepository) : IIncidentQueryService
{
    /// <summary>
    /// Retrieves the active incidents belonging to the clinic specified by the query.
    /// </summary>
    /// <param name="query">The clinic used to filter active incidents.</param>
    /// <param name="cancellationToken">The token used to cancel the repository operation.</param>
    /// <returns>The matching active incidents.</returns>
    public async Task<IEnumerable<Incident>> Handle(GetActiveIncidentsQuery query, CancellationToken cancellationToken = default)
    {
        return await incidentRepository.FindActiveIncidentsAsync(query.ClinicId, cancellationToken);
    }

    /// <summary>
    /// Retrieves every persisted incident.
    /// </summary>
    /// <param name="query">The query requesting the complete incident list.</param>
    /// <param name="cancellationToken">The token used to cancel the repository operation.</param>
    /// <returns>All persisted incidents.</returns>
    public async Task<IEnumerable<Incident>> Handle(ListAllIncidentsQuery query, CancellationToken cancellationToken = default)
    {
        return await incidentRepository.ListAsync();
    }

    /// <summary>
    /// Retrieves an incident by its identifier.
    /// </summary>
    /// <param name="query">The query containing the incident identifier.</param>
    /// <param name="cancellationToken">The token used to cancel the repository operation.</param>
    /// <returns>The matching incident, or <see langword="null"/> when it does not exist.</returns>
    public async Task<Incident?> Handle(GetIncidentByIdQuery query, CancellationToken cancellationToken = default)
    {
        return await incidentRepository.GetByIdAsync(query.IncidentId, cancellationToken);
    }
}