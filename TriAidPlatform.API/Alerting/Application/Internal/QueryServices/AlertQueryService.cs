using TriAidPlatform.API.Alerting.Domain.Model.Aggregates;
using TriAidPlatform.API.Alerting.Domain.Model.Queries;
using TriAidPlatform.API.Alerting.Domain.Repositories;

namespace TriAidPlatform.API.Alerting.Application.Internal.QueryServices;

/// <summary>
/// Service responsible for handling read-only queries related to the Alert aggregate.
/// Utilizes a primary constructor to inject the required data access dependencies.
/// </summary>
/// <param name="alertRepository">The repository interface used to query alert data from the database.</param>
public class AlertQueryService(IAlertRepository alertRepository)
{
    /// <summary>
    /// Handles the execution of the <see cref="GetActiveAlertsQuery"/> to retrieve active alerts.
    /// Enforces multi-tenant data isolation by filtering alerts strictly based on the provided ClinicId.
    /// </summary>
    /// <param name="query">The query object containing the ClinicId constraint.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>An asynchronous task returning a collection of active <see cref="Alert"/> instances for the specified clinic.</returns>
    public async Task<IEnumerable<Alert>> Handle(GetActiveAlertsQuery query, CancellationToken cancellationToken = default)
    {
        
        return await alertRepository.FindActiveAlertsAsync(query.ClinicId, cancellationToken);
    }
}