using TriAidPlatform.API.Iam.Domain.Model.Aggregates;
using TriAidPlatform.API.Iam.Domain.Model.ValueObjects;
using TriAidPlatform.API.Shared.Domain.Repositories;

namespace TriAidPlatform.API.Iam.Domain.Repositories;

/**
 * <summary>
 *     The user repository
 * </summary>
 * <remarks>
 *     This repository is used to manage users
 * </remarks>
 */
public interface IUserRepository : IBaseRepository<User>
{
    /**
     * <summary>
     *     Find a user by username
     * </summary>
     * <param name="username">The username to search</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The user</returns>
     */
    Task<User?> FindByUsernameAsync(string username, CancellationToken cancellationToken);

    /**
     * <summary>
     *     Find a user by username without change tracking
     * </summary>
     * <param name="username">The username to search</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The user</returns>
     */
    Task<User?> FindByUsernameAsNoTrackingAsync(string username, CancellationToken cancellationToken);

    /**
     * <summary>
     *     Find a user by id without change tracking
     * </summary>
     * <param name="id">The user id to search</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The user</returns>
     */
    Task<User?> FindByIdAsNoTrackingAsync(UserId id, CancellationToken cancellationToken);

    /**
     * <summary>
     *     Check if a user exists by username
     * </summary>
     * <param name="username">The username to search</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>True if the username is taken</returns>
     */
    Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken);

    /**
     * <summary>
     *     List the staff accounts of a tenant institution
     * </summary>
     * <param name="institution">The tenant institution</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The users of the institution</returns>
     */
    Task<IEnumerable<User>> ListByInstitutionAsync(string institution, CancellationToken cancellationToken);

    /**
     * <summary>
     *     Whether the institution already has an administrator
     * </summary>
     * <param name="institution">The tenant institution</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>True if an administrator exists</returns>
     */
    Task<bool> ExistsAdminByInstitutionAsync(string institution, CancellationToken cancellationToken);

    /**
     * <summary>
     *     Count the accounts of a tenant institution (seat usage)
     * </summary>
     * <param name="institution">The tenant institution</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The number of accounts</returns>
     */
    Task<int> CountByInstitutionAsync(string institution, CancellationToken cancellationToken);
}
