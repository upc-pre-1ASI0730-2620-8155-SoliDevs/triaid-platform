using System.Net.Mime;
using TriAidPlatform.API.Iam.Application.CommandServices;
using TriAidPlatform.API.Iam.Application.QueryServices;
using TriAidPlatform.API.Iam.Domain.Model.Aggregates;
using TriAidPlatform.API.Iam.Domain.Model.Commands;
using TriAidPlatform.API.Iam.Domain.Model.Queries;
using TriAidPlatform.API.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using TriAidPlatform.API.Iam.Interfaces.Rest.Resources;
using TriAidPlatform.API.Iam.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace TriAidPlatform.API.Iam.Interfaces.Rest;

/**
 * <summary>
 *     The users controller
 * </summary>
 * <remarks>
 *     This controller handles staff account management. Reads are
 *     tenant-scoped; writes are restricted to administrators (RBAC).
 * </remarks>
 */
[Authorize]
[ApiController]
[Route("users")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Staff accounts")]
public class UsersController(
    IUserCommandService userCommandService,
    IUserQueryService userQueryService) : ControllerBase
{
    /// <summary>
    ///     Resolves the authenticated user from the request context
    /// </summary>
    private User? CurrentUser() => (User?)HttpContext.Items["User"];

    /**
     * <summary>
     *     List the accounts the caller may see. Administrators see
     *     the staff of their institution; other roles see themselves
     * </summary>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The visible accounts</returns>
     * <response code="200">Visible accounts</response>
     * <response code="401">Missing or invalid token</response>
     */
    [HttpGet]
    [EndpointSummary("List visible staff accounts")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var caller = CurrentUser();
        if (caller is null) return Unauthorized(new { error = "invalid-token" });

        if (caller.IsAdmin)
        {
            var staff = await userQueryService.Handle(
                new GetUsersByInstitutionQuery(caller.Institution), cancellationToken);
            return Ok(staff.Select(UserResourceFromEntityAssembler.ToResourceFromEntity));
        }

        return Ok(new[] { UserResourceFromEntityAssembler.ToResourceFromEntity(caller) });
    }

    /**
     * <summary>
     *     Register a staff account under the caller institution (US55)
     * </summary>
     * <param name="resource">The staff resource</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The created staff account</returns>
     * <response code="200">Account created</response>
     * <response code="401">Missing or invalid token</response>
     * <response code="403">Non-admin caller or plan seat limit reached</response>
     * <response code="409">Username already taken</response>
     */
    [HttpPost]
    [EndpointSummary("Create staff account (admin only)")]
    public async Task<IActionResult> Create([FromBody] StaffResource resource, CancellationToken cancellationToken)
    {
        var caller = CurrentUser();
        if (caller is null) return Unauthorized(new { error = "invalid-token" });

        var command = CreateStaffCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await userCommandService.Handle(caller.Id, command, cancellationToken);

        return result.Match<IActionResult>(
            onSuccess: user => Ok(UserResourceFromEntityAssembler.ToResourceFromEntity(user)),
            onFailure: error => error.Code switch
            {
                "Iam.NotEnoughPermissions" or "Iam.SeatLimitReached" => StatusCode(403, new { error = error.Code }),
                "Iam.UsernameAlreadyTaken" => Conflict(new { error = error.Code }),
                _ => BadRequest(new { error = error.Code })
            });
    }

    /**
     * <summary>
     *     Update an account. Non-admins may only update their own
     *     profile; admins may also change role, shift and unit of
     *     their institution staff. Username and institution are
     *     immutable.
     * </summary>
     * <param name="id">The user identifier to update</param>
     * <param name="resource">The update resource</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The updated account</returns>
     * <response code="200">Account updated</response>
     * <response code="401">Missing or invalid token</response>
     * <response code="403">Trying to edit an account out of scope</response>
     * <response code="404">Account not found</response>
     */
    [HttpPut("{id:int}")]
    [EndpointSummary("Update a user account")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserResource resource,
        CancellationToken cancellationToken)
    {
        var caller = CurrentUser();
        if (caller is null) return Unauthorized(new { error = "invalid-token" });

        var command = UpdateUserCommandFromResourceAssembler.ToCommandFromResource(id, resource);
        var result = await userCommandService.Handle(caller.Id, command, cancellationToken);

        return result.Match<IActionResult>(
            onSuccess: user => Ok(UserResourceFromEntityAssembler.ToResourceFromEntity(user)),
            onFailure: error => error.Code switch
            {
                "Iam.UserNotFound" => NotFound(new { error = error.Code }),
                "Iam.NotEnoughPermissions" => StatusCode(403, new { error = error.Code }),
                _ => BadRequest(new { error = error.Code })
            });
    }
}
