using Microsoft.AspNetCore.Mvc;
using TriAidPlatform.API.Iam.Application.Internal.CommandServices;
using TriAidPlatform.API.Iam.Domain.Model.Aggregates;
using TriAidPlatform.API.Iam.Domain.Repositories;
using TriAidPlatform.API.Iam.Domain.Services;
using TriAidPlatform.API.Iam.Interfaces.REST;
using TriAidPlatform.API.Iam.Interfaces.REST.Resources;
using TriAidPlatform.API.Iam.Interfaces.REST.Transform;
using TriAidPlatform.API.Subscription.Domain.Repositories;
using TriAidPlatform.API.Subscription.Domain.Services;

namespace TriAidPlatform.API.Iam.Interfaces.REST;

/// <summary>
/// REST facade for staff account management. Reads are tenant-scoped;
/// writes are restricted to administrators (RBAC).
/// </summary>
[ApiController]
[Route("users")]
public class UsersController(
    IUserRepository userRepository,
    ISessionRepository sessionRepository,
    IPasswordHasher passwordHasher,
    ISubscriptionQueryService subscriptionQueryService,
    ISubscriptionCommandService subscriptionCommandService) : ControllerBase
{
    /// <summary>Lists the accounts the caller may see (US55).</summary>
    /// <response code="200">Admins see their institution staff; others, themselves.</response>
    /// <response code="401">Missing or invalid token.</response>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var caller = await AuthController.ResolveUserAsync(Request, sessionRepository, userRepository);
        if (caller is null) return Unauthorized(new { error = "invalid-token" });

        var scope = caller.IsAdmin
            ? await userRepository.ListByInstitutionAsync(caller.Institution, cancellationToken)
            : [caller];
        return Ok(scope.Select(UserResourceFromEntityAssembler.ToResource));
    }

    /// <summary>Registers a staff account under the caller's institution (US55).</summary>
    /// <response code="200">Account created.</response>
    /// <response code="401">Missing or invalid token.</response>
    /// <response code="403">Non-admin caller or plan seat limit reached.</response>
    /// <response code="409">Username already taken.</response>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StaffResource resource, CancellationToken cancellationToken)
    {
        var caller = await AuthController.ResolveUserAsync(Request, sessionRepository, userRepository);
        if (caller is null) return Unauthorized(new { error = "invalid-token" });
        if (!caller.IsAdmin) return StatusCode(403, new { error = "admin-only" });
        if (string.IsNullOrWhiteSpace(resource.Username) || string.IsNullOrWhiteSpace(resource.Names))
            return BadRequest(new { error = "username-and-names-required" });

        // Plan seat limit: MaxUsers 0 means unlimited.
        var plan = await subscriptionQueryService.FindCurrentPlanAsync(caller.Institution, cancellationToken);
        var seats = await userRepository.CountByInstitutionAsync(caller.Institution, cancellationToken);
        if (plan is { MaxUsers: > 0 } && seats >= plan.MaxUsers)
            return StatusCode(403, new { error = "seat-limit-reached", limit = plan.MaxUsers });

        if (await userRepository.ExistsUsernameAsync(resource.Username, cancellationToken))
            return Conflict(new { error = "username-taken" });

        var staff = new User
        {
            Username = resource.Username.Trim(),
            PasswordHash = passwordHasher.HashPassword(
                string.IsNullOrWhiteSpace(resource.Password) ? "triaid123" : resource.Password),
            Names = resource.Names,
            Surnames = resource.Surnames,
            Email = resource.Email ?? string.Empty,
            Role = string.IsNullOrWhiteSpace(resource.Role) ? "Triage Nurse" : resource.Role,
            Unit = resource.Unit ?? string.Empty,
            Shift = resource.Shift ?? string.Empty,
            Institution = caller.Institution
        };
        await userRepository.AddAsync(staff, cancellationToken);
        await userRepository.CompleteAsync(cancellationToken);
        return Ok(UserResourceFromEntityAssembler.ToResource(staff));
    }

    /// <summary>
    /// Updates an account. Non-admins may only update their own profile
    /// (names, surnames, email, password); admins may also change the
    /// role, shift and unit of their institution staff. Username and
    /// institution are immutable.
    /// </summary>
    /// <response code="200">Account updated.</response>
    /// <response code="401">Missing or invalid token.</response>
    /// <response code="403">Trying to edit an account out of scope.</response>
    /// <response code="404">Account not found.</response>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserResource resource, CancellationToken cancellationToken)
    {
        var caller = await AuthController.ResolveUserAsync(Request, sessionRepository, userRepository);
        if (caller is null) return Unauthorized(new { error = "invalid-token" });

        var target = await userRepository.FindByIdAsync(id, cancellationToken);
        if (target is null) return NotFound();
        if (!caller.IsAdmin && target.Id != caller.Id) return StatusCode(403, new { error = "self-profile-only" });

        // Profile fields: any authorized caller may change them.
        if (!string.IsNullOrWhiteSpace(resource.Names)) target.Names = resource.Names;
        if (!string.IsNullOrWhiteSpace(resource.Surnames)) target.Surnames = resource.Surnames;
        if (resource.Email is not null) target.Email = resource.Email;
        if (!string.IsNullOrWhiteSpace(resource.Password))
            target.PasswordHash = passwordHasher.HashPassword(resource.Password);

        // Admin-only fields; an admin never edits another admin.
        if (caller.IsAdmin && !target.IsAdmin)
        {
            if (!string.IsNullOrWhiteSpace(resource.Role)) target.Role = resource.Role;
            if (!string.IsNullOrWhiteSpace(resource.Shift)) target.Shift = resource.Shift;
            if (!string.IsNullOrWhiteSpace(resource.Unit)) target.Unit = resource.Unit;
        }

        await userRepository.CompleteAsync(cancellationToken);
        return Ok(UserResourceFromEntityAssembler.ToResource(target));
    }
}
