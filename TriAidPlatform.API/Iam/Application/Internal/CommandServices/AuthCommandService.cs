using TriAidPlatform.API.Iam.Domain.Model.Aggregates;
using TriAidPlatform.API.Iam.Domain.Repositories;
using TriAidPlatform.API.Iam.Domain.Services;
using TriAidPlatform.API.Iam.Interfaces.REST.Resources;
using TriAidPlatform.API.Subscription.Domain.Services;

namespace TriAidPlatform.API.Iam.Application.Internal.CommandServices;

/// <summary>
/// Handles authentication and onboarding use cases: sign-in, sign-out,
/// session restore and the institution self-service registration
/// (administrator + trial subscription in one transaction).
/// </summary>
public class AuthCommandService(
    IUserRepository userRepository,
    ISessionRepository sessionRepository,
    IPasswordHasher passwordHasher,
    ISubscriptionCommandService subscriptionCommandService)
{
    /// <summary>Result of an authentication operation.</summary>
    /// <param name="Ok">Whether the operation succeeded.</param>
    /// <param name="Error">Machine-readable error code, when failed.</param>
    /// <param name="Token">Bearer session token, when a session opened.</param>
    /// <param name="User">Affected user projection, when applicable.</param>
    /// <param name="Trial">Trial subscription created during registration.</param>
    public record AuthResult(
        bool Ok, string? Error, string? Token, User? User,
        Subscription.Domain.Services.TrialResult? Trial = null);

    /// <summary>Signs in with username and password.</summary>
    public async Task<AuthResult> LoginAsync(LoginResource resource, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.FindByUsernameAsync(resource.Username, cancellationToken);
        if (user is null || !passwordHasher.VerifyPassword(resource.Password, user.PasswordHash))
            return new AuthResult(false, "invalid-credentials", null, null);

        var session = new Session { UserId = user.Id, Username = user.Username };
        await sessionRepository.AddAsync(session, cancellationToken);
        await sessionRepository.CompleteAsync(cancellationToken);
        return new AuthResult(true, null, session.Id, user);
    }

    /// <summary>
    /// Registers an institution together with its administrator and
    /// starts the card-required trial subscription.
    /// </summary>
    public async Task<AuthResult> RegisterAsync(RegisterResource resource, CancellationToken cancellationToken = default)
    {
        var username = resource.Username.Trim();
        var institution = resource.Institution.Trim();

        if (await userRepository.ExistsUsernameAsync(username, cancellationToken))
            return new AuthResult(false, "username-taken", null, null);
        var existingStaff = await userRepository.ListByInstitutionAsync(institution, cancellationToken);
        if (existingStaff.Any())
            return new AuthResult(false, "institution-taken", null, null);

        var admin = new User
        {
            Username = username,
            PasswordHash = passwordHasher.HashPassword(resource.Password),
            Names = resource.Names,
            Surnames = resource.Surnames,
            Email = resource.Email ?? string.Empty,
            Role = "Triage Administrator",
            Unit = "Triaje",
            Institution = institution
        };
        await userRepository.AddAsync(admin, cancellationToken);

        var trial = await subscriptionCommandService.StartTrialAsync(
            institution, resource.PlanId ?? "plan-profesional", resource.CardNumber ?? string.Empty,
            cancellationToken);

        var session = new Session { UserId = admin.Id, Username = admin.Username };
        await sessionRepository.AddAsync(session, cancellationToken);
        await userRepository.CompleteAsync(cancellationToken);

        return new AuthResult(true, null, session.Id, admin, trial);
    }

    /// <summary>Closes an open session.</summary>
    public async Task LogoutAsync(string token, CancellationToken cancellationToken = default)
    {
        var session = await sessionRepository.FindByIdAsync(token, cancellationToken);
        if (session is null) return;
        sessionRepository.Remove(session);
        await sessionRepository.CompleteAsync(cancellationToken);
    }
}
