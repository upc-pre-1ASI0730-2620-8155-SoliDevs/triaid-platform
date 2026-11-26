using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TriAidPlatform.API.Iam.Infrastructure.Pipeline.Middleware.Attributes;

/// <summary>
/// Guards endpoints that serve the patient portal. Requires a valid patient
/// token (role=Patient) and rejects staff tokens: the two portals never share access.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizePatientAttribute : Attribute, IAuthorizationFilter
{
    /// <summary>Rejects the request with 401 when no patient session is attached.</summary>
    /// <param name="context">The authorization filter context.</param>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (context.HttpContext.Items.ContainsKey("Patient")) return;
        context.Result = new UnauthorizedResult();
    }
}
