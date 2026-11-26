using TriAidPlatform.API.Incidents.Domain.Model.Aggregates;
using TriAidPlatform.API.Incidents.Domain.Model.Queries;

namespace TriAidPlatform.API.Incidents.Application.QueryServices;

/// <summary>
/// Defines read-only operations for retrieving incidents.
/// </summary>
public interface IIncidentQueryService
{
    /// <summary>
    /// Retrieves incidents that remain active within the specified clinic.
    /// </summary>
    /// <param name="query">The clinic used to filter active incidents.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>The matching active incidents.</returns>
    Task<IEnumerable<Incident>> Handle(GetActiveIncidentsQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all incidents without applying a clinic filter.
    /// </summary>
    /// <param name="query">The query requesting the complete incident list.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>All persisted incidents.</returns>
    Task<IEnumerable<Incident>> Handle(ListAllIncidentsQuery query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an incident using its identifier.
    /// </summary>
    /// <param name="query">The query containing the incident identifier.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>The matching incident, or <see langword="null"/> when it does not exist.</returns>
    Task<Incident?> Handle(GetIncidentByIdQuery query, CancellationToken cancellationToken = default);
}