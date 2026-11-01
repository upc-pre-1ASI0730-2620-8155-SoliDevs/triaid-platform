using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.Alerting.Domain.Model.Aggregates;
using TriAidPlatform.API.Alerting.Domain.Model.ValueObjects;
using TriAidPlatform.API.Alerting.Domain.Repositories;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace TriAidPlatform.API.Alerting.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
/// Infrastructure implementation of the <see cref="IAlertRepository"/> interface.
/// Provides data access operations for the <see cref="Alert"/> aggregate using Entity Framework Core.
/// Acts as a bridge between the domain rules of the Alerting bounded context and the relational database.
/// </summary>
public class AlertRepository : BaseRepository<Alert>, IAlertRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AlertRepository"/> class.
    /// </summary>
    /// <param name="context">The database context provided via dependency injection.</param>
    public AlertRepository(AppDbContext context) : base(context) { }
    
    /// <summary>
    /// Retrieves all active alerts from the database. An active alert is any alert
    /// that has not yet reached the 'Resolved' state. This query is primarily used
    /// to populate the Triage Queue and the Alert Center UI.
    /// </summary>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>An asynchronous task returning a collection of active <see cref="Alert"/> instances.</returns>
    public async Task<IEnumerable<Alert>> FindActiveAlertsAsync(CancellationToken cancellationToken = default)
    {
        
        return await Context.Set<Alert>()
            .Where(a => a.Status.StatusValue != EAlertStatus.Resolved)
            .ToListAsync(cancellationToken);
    }
    
    /// <summary>
    /// Retrieves a specific alert based on its associated reading ID.
    /// This method is crucial to prevent duplicate alerts for the same vital sign anomaly.
    /// </summary>
    /// <param name="readingId">The unique identifier of the vital sign reading.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>
    /// An asynchronous task returning the <see cref="Alert"/> instance if found; 
    /// otherwise, null.
    /// </returns>
    public async Task<Alert?> FindByReadingIdAsync(int readingId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Alert>()
            .FirstOrDefaultAsync(a => a.ReadingId == readingId, cancellationToken);
    }
}
