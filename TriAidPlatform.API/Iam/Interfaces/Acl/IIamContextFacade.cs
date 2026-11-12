namespace TriAidPlatform.API.Iam.Interfaces.Acl;

/**
 * <summary>
 *     The IAM anti-corruption layer contract. Other bounded contexts
 *     depend on this facade instead of the IAM internals.
 * </summary>
 */
public interface IIamContextFacade
{
    /**
     * <summary>
     *     Fetch the identifier of a user by username
     * </summary>
     * <param name="username">The username to search</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The user id, or 0 when not found</returns>
     */
    Task<int> FetchUserIdByUsername(string username, CancellationToken cancellationToken);

    /**
     * <summary>
     *     Fetch the username of a user by identifier
     * </summary>
     * <param name="userId">The user identifier</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The username, or an empty string when not found</returns>
     */
    Task<string> FetchUsernameByUserId(int userId, CancellationToken cancellationToken);

    /**
     * <summary>
     *     Fetch the tenant institution of a user by identifier
     * </summary>
     * <param name="userId">The user identifier</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The institution, or an empty string when not found</returns>
     */
    Task<string> FetchInstitutionByUserId(int userId, CancellationToken cancellationToken);
}
