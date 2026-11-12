using TriAidPlatform.API.Iam.Domain.Model.Aggregates;

namespace TriAidPlatform.API.Iam.Application.Internal.OutboundServices;

/**
 * <summary>
 *     The token service interface
 * </summary>
 * <remarks>
 *     This interface is used to generate and validate JWT tokens
 * </remarks>
 */
public interface ITokenService
{
    /**
     * <summary>
     *     Generate a JWT token carrying the user identity and tenant
     * </summary>
     * <param name="user">The user to generate the token for</param>
     * <returns>The generated token</returns>
     */
    string GenerateToken(User user);

    /**
     * <summary>
     *     Validate a JWT token
     * </summary>
     * <param name="token">The token to validate</param>
     * <returns>The user id when valid, null otherwise</returns>
     */
    int? ValidateToken(string token);
}
