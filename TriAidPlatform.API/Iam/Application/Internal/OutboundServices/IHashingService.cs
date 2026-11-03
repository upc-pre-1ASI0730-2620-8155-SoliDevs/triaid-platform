namespace TriAidPlatform.API.Iam.Application.Internal.OutboundServices;

/**
 * <summary>
 *     The hashing service interface
 * </summary>
 * <remarks>
 *     This interface is used to hash and verify passwords
 * </remarks>
 */
public interface IHashingService
{
    /**
     * <summary>
     *     Hash a plain-text password
     * </summary>
     * <param name="password">The plain-text password</param>
     * <returns>The hashed password</returns>
     */
    string HashPassword(string password);

    /**
     * <summary>
     *     Verify a plain-text password against a stored hash
     * </summary>
     * <param name="password">The plain-text password</param>
     * <param name="passwordHash">The stored hash</param>
     * <returns>True when the password matches</returns>
     */
    bool VerifyPassword(string password, string passwordHash);
}
