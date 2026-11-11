using TriAidPlatform.API.Iam.Application.QueryServices;
using TriAidPlatform.API.Iam.Domain.Model.Aggregates;
using TriAidPlatform.API.Iam.Domain.Model.Queries;
using TriAidPlatform.API.Iam.Domain.Repositories;

namespace TriAidPlatform.API.Iam.Application.Internal.QueryServices;

/**
 * <summary>
 *     The user query service implementation class
 * </summary>
 * <remarks>
 *     This class is used to handle user queries
 * </remarks>
 */
public class UserQueryService(IUserRepository userRepository) : IUserQueryService
{
    /**
     * <summary>
     *     Handle get user by id query
     * </summary>
     * <param name="query">The query object containing the user id to search</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The user</returns>
     */
    public async Task<User?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        return await userRepository.FindByIdAsNoTrackingAsync(query.Id, cancellationToken);
    }

    /**
     * <summary>
     *     Handle get all users query
     * </summary>
     * <param name="query">The query object for getting all users</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The users</returns>
     */
    public async Task<IEnumerable<User>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
    {
        return await userRepository.ListAsync(cancellationToken);
    }

    /**
     * <summary>
     *     Handle get user by username query
     * </summary>
     * <param name="query">The query object containing the username to search</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The user</returns>
     */
    public async Task<User?> Handle(GetUserByUsernameQuery query, CancellationToken cancellationToken)
    {
        return await userRepository.FindByUsernameAsNoTrackingAsync(query.Username, cancellationToken);
    }

    /**
     * <summary>
     *     Handle get users by institution query
     * </summary>
     * <param name="query">The query object containing the tenant institution</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The staff accounts of the institution</returns>
     */
    public async Task<IEnumerable<User>> Handle(GetUsersByInstitutionQuery query, CancellationToken cancellationToken)
    {
        return await userRepository.ListByInstitutionAsync(query.Institution, cancellationToken);
    }
}
