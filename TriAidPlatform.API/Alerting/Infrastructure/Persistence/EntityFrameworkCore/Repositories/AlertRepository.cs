using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.Alerting.Domain.Model.Aggregates;
using TriAidPlatform.API.Alerting.Domain.Model.ValueObjects;
using TriAidPlatform.API.Alerting.Domain.Repositories;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace TriAidPlatform.API.Alerting.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class AlertRepository : BaseRepository<Alert>, IAlertRepository
{
    public AlertRepository(AppDbContext context) : base(context) { }
    
    public async Task<IEnumerable<Alert>> FindActiveAlertsAsync(CancellationToken cancellationToken = default)
    {
        
        return await Context.Set<Alert>()
            .Where(a => a.Status.StatusValue != EAlertStatus.Resolved)
            .ToListAsync(cancellationToken);
    }
    
    public async Task<Alert?> FindByReadingIdAsync(int readingId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Alert>()
            .FirstOrDefaultAsync(a => a.ReadingId == readingId, cancellationToken);
    }
}
