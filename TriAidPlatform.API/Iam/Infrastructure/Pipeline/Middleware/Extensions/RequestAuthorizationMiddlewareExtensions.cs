namespace TriAidPlatform.API.Iam.Infrastructure.Pipeline.Middleware.Extensions;

/**
 * Extension method to register the RequestAuthorizationMiddleware
 * in the application request pipeline.
 */
public static class RequestAuthorizationMiddlewareExtensions
{
    /**
     * <summary>
     *     Use the RequestAuthorizationMiddleware
     * </summary>
     * <param name="builder">The application builder</param>
     * <returns>The application builder with the middleware registered</returns>
     */
    public static IApplicationBuilder UseRequestAuthorization(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<Components.RequestAuthorizationMiddleware>();
    }
}
