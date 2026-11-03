using TriAidPlatform.API.Iam.Domain.Model.Aggregates;

namespace TriAidPlatform.API.Iam.Domain.Repositories;

/// <summary>Repository contract for the User aggregate.</summary>
public interface IUserRepository
{
    /// <summary>Finds a user by exact username (case-insensitive).</summary>
    Task<User?> FindByUsernameAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>Finds a user by identifier.</summary>
    Task<User?> FindByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Lists users scoped to an institution.</summary>
    Task<IEnumerable<User>> ListByInstitutionAsync(string institution, CancellationToken cancellationToken = default);

    /// <summary>Whether an institution already has an administrator.</summary>
    Task<bool> ExistsAdminAsync(string institution, CancellationToken cancellationToken = default);

    /// <summary>Whether the username is already registered.</summary>
    Task<bool> ExistsUsernameAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>Counts the accounts of an institution (seat usage).</summary>
    Task<int> CountByInstitutionAsync(string institution, CancellationToken cancellationToken = default);

    /// <summary>Inserts a new user.</summary>
    Task AddAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>Persists changes.</summary>
    Task CompleteAsync(CancellationToken cancellationToken = default);
}
