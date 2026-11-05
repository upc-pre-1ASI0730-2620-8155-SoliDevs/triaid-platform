using System.Net.Mime;
using TriAidPlatform.API.Iam.Application.CommandServices;
using TriAidPlatform.API.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using TriAidPlatform.API.Iam.Interfaces.Rest.Resources;
using TriAidPlatform.API.Iam.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;
using TriAidPlatform.API.Shared.Domain.Model.Errors;

namespace TriAidPlatform.API.Iam.Interfaces.Rest;

/**
 * <summary>
 *     The authentication controller
 * </summary>
 * <remarks>
 *     This controller is used to handle authentication requests:
 *     sign in, institution registration, sign out and session
 *     restore. Routes are kept compatible with the web application.
 * </remarks>
 */
[ApiController]
[Route("auth")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Authentication")]
public class AuthenticationController(IUserCommandService userCommandService) : ControllerBase
{
    /**
     * <summary>
     *     Sign in endpoint. It allows authenticating a user and
     *     obtaining the JWT bearer token
     * </summary>
     * <param name="signInResource">The sign-in resource containing username and password</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The authenticated user resource, including the token</returns>
     * <response code="200">Authenticated</response>
     * <response code="401">Invalid credentials</response>
     */
    [HttpPost("login")]
    [AllowAnonymous]
    [EndpointSummary("Sign in")]
    [ProducesResponseType(typeof(AuthenticatedUserResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SignIn([FromBody] SignInResource signInResource,
        CancellationToken cancellationToken)
    {
        var signInCommand = SignInCommandFromResourceAssembler.ToCommandFromResource(signInResource);
        var result = await userCommandService.Handle(signInCommand, cancellationToken);

        return result.Match<IActionResult>(
            onSuccess: userAndToken =>
                Ok(AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(userAndToken.user,
                    userAndToken.token)),
            onFailure: error => Unauthorized(new { error = error.Code }));
    }

    /**
     * <summary>
     *     Register institution endpoint. It creates the administrator
     *     account, joins it to the new institution and starts the
     *     14-day card-required trial (US54)
     * </summary>
     * <param name="resource">The registration resource</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The authenticated administrator with the trial subscription</returns>
     * <response code="200">Onboarding completed</response>
     * <response code="409">Username or institution already registered</response>
     */
    [HttpPost("register")]
    [AllowAnonymous]
    [EndpointSummary("Register institution with its administrator")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] InstitutionRegisterResource resource,
        CancellationToken cancellationToken)
    {
        var command = RegisterInstitutionCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await userCommandService.Handle(command, cancellationToken);

        return result.Match<IActionResult>(
            onSuccess: userAndToken =>
                Ok(AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(userAndToken.user,
                    userAndToken.token)),
            onFailure: error => Conflict(new { error = error.Code }));
    }

    /**
     * <summary>
     *     Sign out endpoint. JWT sessions are stateless, so the client
     *     discards the token; the endpoint exists to honor the
     *     application contract
     * </summary>
     * <response code="200">Signed out</response>
     */
    [HttpPost("logout")]
    [EndpointSummary("Sign out")]
    public IActionResult SignOut()
    {
        // JWT sessions are stateless: the client discards the token.
        return Ok(new { ok = true });
    }

    /**
     * <summary>
     *     Me endpoint. It returns the profile of the authenticated user
     * </summary>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The user resource</returns>
     * <response code="200">Authenticated</response>
     * <response code="401">Missing or invalid token</response>
     */
    [HttpGet("me")]
    [Authorize]
    [EndpointSummary("Current session profile")]
    public IActionResult Me()
    {
        var user = (Domain.Model.Aggregates.User?)HttpContext.Items["User"];
        if (user is null) return Unauthorized(new { error = "invalid-token" });
        return Ok(UserResourceFromEntityAssembler.ToResourceFromEntity(user));
    }
}
