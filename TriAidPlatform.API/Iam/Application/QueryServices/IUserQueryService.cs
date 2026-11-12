using TriAidPlatform.API.Iam.Domain.Model.Aggregates;
using TriAidPlatform.API.Iam.Domain.Model.Queries;

namespace TriAidPlatform.API.Iam.Application.QueryServices;

/**
 * <summary>
 *     The user query service interface
 * </summary>
 * <remarks>
 *     This interface is used to handle user queries
 * </remarks>
 */
public interface IUserQueryService
{
    /**
     * <summary>
     *     Handle the get user by id query
     * </summary>
     */
    Task<User?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken);

    /**
     * <summary>
     *     Handle the get all users query
     * </summary>
     */
    Task<IEnumerable<User>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken);

    /**
     * <summary>
     *     Handle the get user by username query
     * </summary>
     */
    Task<User?> Handle(GetUserByUsernameQuery query, CancellationToken cancellationToken);

    /**
     * <summary>
     *     Handle the get users by institution query (multi-tenant)
     * </summary>
     */
    Task<IEnumerable<User>> Handle(GetUsersByInstitutionQuery query, CancellationToken cancellationToken);
}
