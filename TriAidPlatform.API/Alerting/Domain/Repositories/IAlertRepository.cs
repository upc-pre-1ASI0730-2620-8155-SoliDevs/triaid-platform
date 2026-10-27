using TriAidPlatform.API.Alerting.Domain.Model.Aggregates;
using TriAidPlatform.API.Shared.Domain.Repositories;

namespace TriAidPlatform.API.Alerting.Domain.Repositories;

/// <summary>
/// Repository interface for the Alert aggregate root.
/// Inherits basic CRUD operations from IBaseRepository and defines specific data access methods for alerts.
/// </summary>
public interface IAlertRepository : IBaseRepository <Alert>
{
    /// <summary>
    /// Retrieves all active alerts (not resolved) from the database.
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>A collection of active <see cref="Alert"/> instances.</returns>
    Task<IEnumerable<Alert>> FindActiveAlertsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves a specific alert based on its associated reading ID.
    /// </summary>
    /// <param name="readingId">The ID of the vital sign reading.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The <see cref="Alert"/> instance if found; otherwise, null.</returns>
    Task<Alert?> FindByReadingIdAsync(int readingId, CancellationToken cancellationToken = default);
} 