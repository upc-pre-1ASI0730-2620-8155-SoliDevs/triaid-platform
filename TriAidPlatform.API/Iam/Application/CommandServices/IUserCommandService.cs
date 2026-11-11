using TriAidPlatform.API.Iam.Domain.Model.Aggregates;
using TriAidPlatform.API.Iam.Domain.Model.Commands;
using TriAidPlatform.API.Shared.Application.Model;

namespace TriAidPlatform.API.Iam.Application.CommandServices;

/**
 * <summary>
 *     The user command service interface
 * </summary>
 * <remarks>
 *     This interface is used to handle user commands
 * </remarks>
 */
public interface IUserCommandService
{
    /**
     * <summary>
     *     Handle the sign in command: authenticates credentials and
     *     issues the JWT bearer token
     * </summary>
     * <param name="command">The sign in command</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The authenticated user and the token</returns>
     */
    Task<Result<(User user, string token)>> Handle(SignInCommand command, CancellationToken cancellationToken);

    /**
     * <summary>
     *     Handle the sign up command: creates a plain account
     * </summary>
     * <param name="command">The sign up command</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The created user</returns>
     */
    Task<Result<User>> Handle(SignUpCommand command, CancellationToken cancellationToken);

    /**
     * <summary>
     *     Handle the register institution command: creates the
     *     administrator account, joins it to the new institution and
     *     starts the trial subscription
     * </summary>
     * <param name="command">The register institution command</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The created administrator and the token</returns>
     */
    Task<Result<(User user, string token)>> Handle(RegisterInstitutionCommand command, CancellationToken cancellationToken);

    /**
     * <summary>
     *     Handle the create staff command: registers a staff account
     *     inheriting the caller institution (admin-only)
     * </summary>
     * <param name="callerId">The acting user identifier</param>
     * <param name="command">The create staff command</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The created staff user</returns>
     */
    Task<Result<User>> Handle(int callerId, CreateStaffCommand command, CancellationToken cancellationToken);

    /**
     * <summary>
     *     Handle the update user command: profile fields for everyone,
     *     assignment fields only for admins over staff accounts
     * </summary>
     * <param name="callerId">The acting user identifier</param>
     * <param name="command">The update user command</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The updated user</returns>
     */
    Task<Result<User>> Handle(int callerId, UpdateUserCommand command, CancellationToken cancellationToken);
}
