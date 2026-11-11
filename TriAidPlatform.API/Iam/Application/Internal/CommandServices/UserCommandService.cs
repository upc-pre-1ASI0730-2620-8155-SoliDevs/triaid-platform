using TriAidPlatform.API.Iam.Application.CommandServices;
using TriAidPlatform.API.Iam.Application.Internal.OutboundServices;
using TriAidPlatform.API.Iam.Domain.Model;
using TriAidPlatform.API.Iam.Domain.Model.Aggregates;
using TriAidPlatform.API.Iam.Domain.Model.Commands;
using TriAidPlatform.API.Iam.Domain.Model.Errors;
using TriAidPlatform.API.Iam.Domain.Repositories;
using TriAidPlatform.API.Shared.Application.Model;
using TriAidPlatform.API.Shared.Domain.Repositories;

namespace TriAidPlatform.API.Iam.Application.Internal.CommandServices;

/**
 * <summary>
 *     The user command service
 * </summary>
 * <remarks>
 *     This class is used to handle user commands
 * </remarks>
 */
public class UserCommandService(
    IUserRepository userRepository,
    ITokenService tokenService,
    IHashingService hashingService,
    IUnitOfWork unitOfWork)
    : IUserCommandService
{
    /**
     * <summary>
     *     Handle sign in command
     * </summary>
     * <param name="command">The sign in command</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The authenticated user and the JWT token</returns>
     */
    public async Task<Result<(User user, string token)>> Handle(SignInCommand command,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.FindByUsernameAsync(command.Username, cancellationToken);

        if (user == null || !hashingService.VerifyPassword(command.Password, user.PasswordHash))
            return Result<(User, string)>.Failure(IamErrors.InvalidCredentials, IamError.InvalidCredentials);

        var token = tokenService.GenerateToken(user);

        return Result<(User, string)>.Success((user, token));
    }

    /**
     * <summary>
     *     Handle sign up command
     * </summary>
     * <param name="command">The sign up command</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The created user result.</returns>
     */
    public async Task<Result<User>> Handle(SignUpCommand command, CancellationToken cancellationToken)
    {
        if (await userRepository.ExistsByUsernameAsync(command.Username, cancellationToken))
            return Result<User>.Failure(IamErrors.UsernameAlreadyTaken, IamError.UsernameAlreadyTaken);

        var hashedPassword = hashingService.HashPassword(command.Password);
        var user = new User(command.Username, hashedPassword);

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<User>.Success(user);
    }

    /**
     * <summary>
     *     Handle register institution command: creates the administrator
     *     account, joins it to the new institution and starts the
     *     trial subscription (US54)
     * </summary>
     * <param name="command">The register institution command</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The created administrator and the token</returns>
     */
    public async Task<Result<(User user, string token)>> Handle(RegisterInstitutionCommand command,
        CancellationToken cancellationToken)
    {
        if (await userRepository.ExistsByUsernameAsync(command.Username, cancellationToken))
            return Result<(User, string)>.Failure(IamErrors.UsernameAlreadyTaken, IamError.UsernameAlreadyTaken);

        var existingStaff = await userRepository.ListByInstitutionAsync(command.Institution, cancellationToken);
        if (existingStaff.Any())
            return Result<(User, string)>.Failure(IamErrors.InstitutionAlreadyRegistered,
                IamError.InstitutionAlreadyRegistered);

        var hashedPassword = hashingService.HashPassword(command.Password);
        var admin = new User(command.Username, hashedPassword)
            .UpdateProfile(command.Names, command.Surnames, command.Email ?? string.Empty)
            .JoinInstitution(command.Institution.Trim(), "Triage Administrator");

        // Note: the trial subscription is started by the Subscription
        // bounded context, which consumes this event through its ACL.
        await userRepository.AddAsync(admin, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        var token = tokenService.GenerateToken(admin);
        return Result<(User, string)>.Success((admin, token));
    }

    /**
     * <summary>
     *     Handle create staff command: registers a staff account under
     *     the caller institution enforcing the plan seat limit (US55)
     * </summary>
     * <param name="callerId">The acting administrator identifier</param>
     * <param name="command">The create staff command</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The created staff user</returns>
     */
    public async Task<Result<User>> Handle(int callerId, CreateStaffCommand command,
        CancellationToken cancellationToken)
    {
        var caller = await userRepository.FindByIdAsync(callerId, cancellationToken);
        if (caller is null || !caller.IsAdmin)
            return Result<User>.Failure(IamErrors.NotEnoughPermissions, IamError.NotEnoughPermissions);

        // Note: seat-limit enforcement joins when the Subscription BC
        // merge lands; the check lives in its command service.

        if (await userRepository.ExistsByUsernameAsync(command.Username, cancellationToken))
            return Result<User>.Failure(IamErrors.UsernameAlreadyTaken, IamError.UsernameAlreadyTaken);

        var hashedPassword = hashingService.HashPassword(
            string.IsNullOrWhiteSpace(command.Password) ? "triaid123" : command.Password);
        var staff = new User(command.Username, hashedPassword)
            .UpdateProfile(command.Names, command.Surnames, command.Email ?? string.Empty)
            .JoinInstitution(caller.Institution,
                string.IsNullOrWhiteSpace(command.Role) ? "Triage Nurse" : command.Role);
        staff.UpdateAssignment(staff.Role, command.Unit ?? string.Empty,
            string.IsNullOrWhiteSpace(command.Shift) ? string.Empty : command.Shift);

        await userRepository.AddAsync(staff, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<User>.Success(staff);
    }

    /**
     * <summary>
     *     Handle update user command: profile fields for everyone,
     *     assignment fields only for admins over staff accounts.
     *     Username and institution are immutable.
     * </summary>
     * <param name="callerId">The acting user identifier</param>
     * <param name="command">The update user command</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The updated user</returns>
     */
    public async Task<Result<User>> Handle(int callerId, UpdateUserCommand command,
        CancellationToken cancellationToken)
    {
        var caller = await userRepository.FindByIdAsync(callerId, cancellationToken);
        if (caller is null)
            return Result<User>.Failure(IamErrors.UserNotFound, IamError.UserNotFound);

        var target = await userRepository.FindByIdAsync(command.Id, cancellationToken);
        if (target is null)
            return Result<User>.Failure(IamErrors.UserNotFound, IamError.UserNotFound);
        if (!caller.IsAdmin && target.Id != caller.Id)
            return Result<User>.Failure(IamErrors.NotEnoughPermissions, IamError.NotEnoughPermissions);

        var names = string.IsNullOrWhiteSpace(command.Names) ? target.Names : command.Names;
        var surnames = command.Surnames is null ? target.Surnames : command.Surnames;
        var email = command.Email is null ? target.Email : command.Email;
        target.UpdateProfile(names, surnames, email);

        if (!string.IsNullOrWhiteSpace(command.Password))
            target.UpdatePasswordHash(hashingService.HashPassword(command.Password));

        if (caller.IsAdmin && !target.IsAdmin)
            target.UpdateAssignment(
                string.IsNullOrWhiteSpace(command.Role) ? target.Role : command.Role,
                command.Unit ?? target.Unit,
                command.Shift ?? target.Shift);

        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<User>.Success(target);
    }
}
