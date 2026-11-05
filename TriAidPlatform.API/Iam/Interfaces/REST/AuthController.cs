using Microsoft.AspNetCore.Mvc;
using TriAidPlatform.API.Iam.Application.Internal.CommandServices;
using TriAidPlatform.API.Iam.Domain.Model.Aggregates;
using TriAidPlatform.API.Iam.Domain.Repositories;
using TriAidPlatform.API.Iam.Interfaces.REST.Resources;
using TriAidPlatform.API.Iam.Interfaces.REST.Transform;
using TriAidPlatform.API.Subscription.Domain.Services;

namespace TriAidPlatform.API.Iam.Interfaces.REST;

/// <summary>REST facade for authentication and onboarding use cases.</summary>
[ApiController]
[Route("auth")]
public class AuthController(
    AuthCommandService authCommandService,
    ISessionRepository sessionRepository,
    IUserRepository userRepository) : ControllerBase
{
    /// <summary>Resolves the authenticated user from the Bearer token.</summary>
    internal static async Task<User?> ResolveUserAsync(
        HttpRequest request, ISessionRepository sessions, IUserRepository users)
    {
        var token = (request.Headers.Authorization.FirstOrDefault() ?? "").Replace("Bearer ", "").Trim();
        if (token.Length == 0) return null;
        var session = await sessions.FindByIdAsync(token);
        if (session is null) return null;
        return await users.FindByIdAsync(session.UserId);
    }

    /// <summary>Signs in and returns the session token with the user.</summary>
    /// <response code="200">Authenticated.</response>
    /// <response code="401">Invalid credentials.</response>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginResource resource, CancellationToken cancellationToken)
    {
        var result = await authCommandService.LoginAsync(resource, cancellationToken);
        if (!result.Ok)
            return Unauthorized(new { error = result.Error });

        var user = UserResourceFromEntityAssembler.ToResource(result.User!);
        return Ok(new { token = result.Token, user });
    }

    /// <summary>
    /// Registers an institution with its administrator and starts the
    /// 14-day card-required trial (US54).
    /// </summary>
    /// <response code="200">Onboarding completed.</response>
    /// <response code="409">Username or institution already registered.</response>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterResource resource, CancellationToken cancellationToken)
    {
        var result = await authCommandService.RegisterAsync(resource, cancellationToken);
        if (!result.Ok)
            return Conflict(new { error = result.Error });

        var user = UserResourceFromEntityAssembler.ToResource(result.User!);
        return Ok(new { token = result.Token, user, subscription = result.Trial });
    }

    /// <summary>Closes the session of the caller.</summary>
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var token = (Request.Headers.Authorization.FirstOrDefault() ?? "").Replace("Bearer ", "").Trim();
        await authCommandService.LogoutAsync(token, cancellationToken);
        return Ok(new { ok = true });
    }

    /// <summary>Returns the profile of the authenticated user.</summary>
    /// <response code="200">Authenticated.</response>
    /// <response code="401">Missing or invalid token.</response>
    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var user = await AuthController.ResolveUserAsync(Request, sessionRepository, userRepository);
        return user is null
            ? Unauthorized(new { error = "invalid-token" })
            : Ok(UserResourceFromEntityAssembler.ToResource(user));
    }
}
