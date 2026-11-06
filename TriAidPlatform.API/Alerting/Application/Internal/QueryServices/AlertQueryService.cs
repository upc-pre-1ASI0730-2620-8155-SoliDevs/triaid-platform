using TriAidPlatform.API.Alerting.Domain.Model.Aggregates;
using TriAidPlatform.API.Alerting.Domain.Model.Queries;
using TriAidPlatform.API.Alerting.Domain.Repositories;

namespace TriAidPlatform.API.Alerting.Application.Internal.QueryServices;

public class AlertQueryService(IAlertRepository alertRepository)
{
    public async Task<IEnumerable<Alert>> Handle(GetActiveAlertsQuery query, CancellationToken cancellationToken = default)
    {
        
        return await alertRepository.FindActiveAlertsAsync(query.ClinicId, cancellationToken);
    }
}