using TriAidPlatform.API.Iam.Application.Internal.OutboundServices;
using BCryptNet = BCrypt.Net.BCrypt;

namespace TriAidPlatform.API.Iam.Infrastructure.Hashing.BCrypt.Services;

/**
 * <summary>
 *     The hashing service implementation using BCrypt
 * </summary>
 * <remarks>
 *     This class is used to hash and verify passwords with BCrypt
 * </remarks>
 */
public class HashingService : IHashingService
{
    /**
     * <summary>
     *     Hash a plain-text password
     * </summary>
     */
    public string HashPassword(string password) =>
        BCryptNet.HashPassword(password);

    /**
     * <summary>
     *     Verify a plain-text password against a stored hash
     * </summary>
     */
    public bool VerifyPassword(string password, string passwordHash) =>
        BCryptNet.Verify(password, passwordHash);
}
