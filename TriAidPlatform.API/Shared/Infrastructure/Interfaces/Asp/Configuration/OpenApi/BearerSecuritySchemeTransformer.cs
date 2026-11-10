using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace TriAidPlatform.API.Shared.Infrastructure.Interfaces.Asp.Configuration.OpenApi;

/// <summary>
///     Adds the Bearer security scheme to the OpenAPI document so Scalar
///     requests the IAM token before calling protected endpoints.
/// </summary>
public sealed class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Enter the IAM bearer token."
        };

        return Task.CompletedTask;
    }
}
