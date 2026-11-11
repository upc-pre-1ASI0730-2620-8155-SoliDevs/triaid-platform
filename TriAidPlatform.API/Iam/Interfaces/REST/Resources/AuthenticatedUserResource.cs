using TriAidPlatform.API.Iam.Domain.Model.Aggregates;

namespace TriAidPlatform.API.Iam.Interfaces.Rest.Resources;

/**
 * <summary>
 *     The user resource
 * </summary>
 * <remarks>
 *     This resource is used to represent a user without credentials
 * </remarks>
 */
public record UserResource(
    int Id,
    string Username,
    string Names,
    string Surnames,
    string Email,
    string Role,
    string Unit,
    string Shift,
    string Institution);

/**
 * <summary>
 *     The authenticated user resource
 * </summary>
 * <remarks>
 *     This resource is used to represent an authenticated user with
 *     its JWT bearer token
 * </remarks>
 */
public record AuthenticatedUserResource(UserResource User, string Token);
