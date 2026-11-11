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
 *     This controller handles staff account management. Reads expose
 *     the whole listing so the web client can resolve credentials and
 *     tenancy client-side; writes stay tenant-scoped by institution
 *     (ADR-010 backend swap contract).
 * </remarks>
 */
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
     *     List every staff account across institutions
     * </summary>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>All accounts</returns>
     * <response code="200">All accounts</response>
     */
    [HttpGet]
    [EndpointSummary("List all staff accounts")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var staff = await userQueryService.Handle(new GetAllUsersQuery(), cancellationToken);
        return Ok(staff.Select(UserResourceFromEntityAssembler.ToResourceFromEntity));
    }

    /**
     * <summary>
     *     Get a single account by identifier
     * </summary>
     * <param name="id">The user identifier</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The account</returns>
     * <response code="200">Account found</response>
     * <response code="404">Account not found</response>
     */
    [HttpGet("{id:int}")]
    [EndpointSummary("Get a staff account")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var user = await userQueryService.Handle(new GetUserByIdQuery(id), cancellationToken);
        return user is null
            ? NotFound(new { error = "Iam.UserNotFound" })
            : Ok(UserResourceFromEntityAssembler.ToResourceFromEntity(user));
    }

    /**
     * <summary>
     *     Register a staff account. Administrators create accounts
     *     under their institution; anonymous callers register under
     *     the institution sent in the resource (US55, ADR-010)
     * </summary>
     * <param name="resource">The staff resource</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The created staff account</returns>
     * <response code="200">Account created</response>
     * <response code="400">Missing institution</response>
     * <response code="403">Non-admin caller or plan seat limit reached</response>
     * <response code="409">Username already taken</response>
     */
    [HttpPost]
    [EndpointSummary("Create staff account")]
    public async Task<IActionResult> Create([FromBody] StaffResource resource, CancellationToken cancellationToken)
    {
        var caller = CurrentUser();
        var command = CreateStaffCommandFromResourceAssembler.ToCommandFromResource(resource);

        var result = caller is not null
            ? await userCommandService.Handle(caller.Id, command, cancellationToken)
            : await userCommandService.Handle(command, resource.Institution ?? string.Empty, cancellationToken);

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
     *     Update an account. Administrators may change role, shift and
     *     unit of their institution staff; anonymous callers update the
     *     target account directly (ADR-010)
     * </summary>
     * <param name="id">The user identifier to update</param>
     * <param name="resource">The update resource</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The updated account</returns>
     * <response code="200">Account updated</response>
     * <response code="403">Trying to edit an account out of scope</response>
     * <response code="404">Account not found</response>
     */
    [HttpPut("{id:int}")]
    [EndpointSummary("Update a user account")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserResource resource,
        CancellationToken cancellationToken)
    {
        var caller = CurrentUser();
        var command = UpdateUserCommandFromResourceAssembler.ToCommandFromResource(id, resource);

        var result = caller is not null
            ? await userCommandService.Handle(caller.Id, command, cancellationToken)
            : await userCommandService.Handle(command, cancellationToken);

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
