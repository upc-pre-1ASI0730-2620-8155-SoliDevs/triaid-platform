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

    /**
     * <summary>
     *     Generate a JWT token for a patient portal session, scoped to one patient
     * </summary>
     * <param name="session">The patient session identity.</param>
     * <returns>The generated token.</returns>
     */
    string GeneratePatientToken(PatientSession session);

    /**
     * <summary>
     *     Validate a patient portal token and return its session, or null when invalid
     * </summary>
     * <param name="token">The bearer token</param>
     * <returns>The patient session when valid, null otherwise</returns>
     */
    PatientSession? ValidatePatientToken(string token);
}
