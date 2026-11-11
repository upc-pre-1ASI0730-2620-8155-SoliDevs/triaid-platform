namespace TriAidPlatform.API.Iam.Infrastructure.Pipeline.Middleware.Attributes;

/**
 * This attribute is used to decorate controllers and actions that skip authorization.
 * It is intended for the authentication endpoints (login, register).
 */
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AllowAnonymousAttribute : Attribute
{
}
