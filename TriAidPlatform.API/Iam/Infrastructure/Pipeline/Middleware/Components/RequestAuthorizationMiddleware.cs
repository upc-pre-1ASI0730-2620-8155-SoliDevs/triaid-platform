using TriAidPlatform.API.Iam.Application.Internal.OutboundServices;
using TriAidPlatform.API.Iam.Application.QueryServices;
using TriAidPlatform.API.Iam.Domain.Model.Queries;
using TriAidPlatform.API.Iam.Infrastructure.Pipeline.Middleware.Attributes;

namespace TriAidPlatform.API.Iam.Infrastructure.Pipeline.Middleware.Components;

/**
 * RequestAuthorizationMiddleware is a custom middleware.
 * This middleware is used to authorize requests.
 * It validates a token is included in the request header and that the token is valid.
 * If the token is valid then it sets the user in HttpContext.Items["User"].
 */
public class RequestAuthorizationMiddleware(RequestDelegate next)
{
    /**
     * InvokeAsync is called by the ASP.NET Core runtime.
     * It is used to authorize requests.
     * It validates a token is included in the request header and that the token is valid.
     * If the token is valid then it sets the user in HttpContext.Items["User"].
     */
    public async Task InvokeAsync(
        HttpContext context,
        IUserQueryService userQueryService,
        ITokenService tokenService)
    {
        // skip authorization if endpoint is decorated with [AllowAnonymous] attribute
        var allowAnonymous = context.Request.HttpContext.GetEndpoint()?.Metadata
            .Any(m => m.GetType() == typeof(AllowAnonymousAttribute)) ?? false;
        if (allowAnonymous)
        {
            // [AllowAnonymous] attribute is set, so skip authorization
            await next(context);
            return;
        }

        // get token from request header
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token))
        {
            // validate token
            var userId = tokenService.ValidateToken(token);

            if (userId != null)
            {
                // get user by id
                var getUserByIdQuery = new GetUserByIdQuery(userId.Value);

                // set user in HttpContext.Items["User"]
                var user = await userQueryService.Handle(getUserByIdQuery, context.RequestAborted);
                context.Items["User"] = user;
            }
        }

        // call next middleware
        await next(context);
    }
}
