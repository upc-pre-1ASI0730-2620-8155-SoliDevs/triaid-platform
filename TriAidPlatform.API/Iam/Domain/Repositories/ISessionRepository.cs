using TriAidPlatform.API.Iam.Domain.Model.Aggregates;

namespace TriAidPlatform.API.Iam.Domain.Repositories;

/// <summary>Repository contract for open sessions.</summary>
public interface ISessionRepository
{
    /// <summary>Finds an open session by its token.</summary>
    Task<Session?> FindByIdAsync(string token, CancellationToken cancellationToken = default);

    /// <summary>Opens a session.</summary>
    Task AddAsync(Session session, CancellationToken cancellationToken = default);

    /// <summary>Closes (removes) a session.</summary>
    void Remove(Session session);

    /// <summary>Persists changes.</summary>
    Task CompleteAsync(CancellationToken cancellationToken = default);
}
