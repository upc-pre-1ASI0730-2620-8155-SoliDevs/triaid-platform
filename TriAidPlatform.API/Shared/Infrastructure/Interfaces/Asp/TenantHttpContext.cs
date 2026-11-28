using Microsoft.AspNetCore.Http;

namespace TriAidPlatform.API.Shared.Infrastructure.Interfaces.Asp;

/// <summary>
/// Reads the tenant of the request from the authenticated staff member.
/// </summary>
public static class TenantHttpContext
{
    /// <summary>
    /// Institution of the signed-in staff member, or null for anonymous and patient callers.
    /// </summary>
    public static string? StaffInstitution(this HttpContext context) =>
        (context.Items["User"] as Iam.Domain.Model.Aggregates.User)?.Institution is { Length: > 0 } institution ? institution : null;
}
