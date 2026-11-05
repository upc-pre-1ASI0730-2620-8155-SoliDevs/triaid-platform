using TriAidPlatform.API.Iam.Application.CommandServices;
using TriAidPlatform.API.Iam.Application.QueryServices;
using TriAidPlatform.API.Iam.Domain.Model.Queries;

using TriAidPlatform.API.Iam.Interfaces.Acl;

namespace TriAidPlatform.API.Iam.Application.Acl;

/**
 * <summary>
 *     The IAM anti-corruption layer facade. Other bounded contexts
 *     depend on this facade instead of the IAM internals.
 * </summary>
 */
public class IamContextFacade(IUserCommandService userCommandService, IUserQueryService userQueryService)
    : IIamContextFacade
{
    /**
     * <summary>
     *     Fetch the identifier of a user by username
     * </summary>
     * <param name="username">The username to search</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The user id, or 0 when not found</returns>
     */
    public async Task<int> FetchUserIdByUsername(string username, CancellationToken cancellationToken)
    {
        var getUserByUsernameQuery = new GetUserByUsernameQuery(username);
        var result = await userQueryService.Handle(getUserByUsernameQuery, cancellationToken);
        return result?.Id ?? 0;
    }

    /**
     * <summary>
     *     Fetch the username of a user by identifier
     * </summary>
     * <param name="userId">The user identifier</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The username, or an empty string when not found</returns>
     */
    public async Task<string> FetchUsernameByUserId(int userId, CancellationToken cancellationToken)
    {
        var getUserByIdQuery = new GetUserByIdQuery(userId);
        var result = await userQueryService.Handle(getUserByIdQuery, cancellationToken);
        return result?.Username ?? string.Empty;
    }

    /**
     * <summary>
     *     Fetch the tenant institution of a user by identifier
     * </summary>
     * <param name="userId">The user identifier</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The institution, or an empty string when not found</returns>
     */
    public async Task<string> FetchInstitutionByUserId(int userId, CancellationToken cancellationToken)
    {
        var getUserByIdQuery = new GetUserByIdQuery(userId);
        var result = await userQueryService.Handle(getUserByIdQuery, cancellationToken);
        return result?.Institution ?? string.Empty;
    }
}
