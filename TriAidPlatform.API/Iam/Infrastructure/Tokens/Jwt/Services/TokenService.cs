using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TriAidPlatform.API.Iam.Application.Internal.OutboundServices;
using TriAidPlatform.API.Iam.Domain.Model.Aggregates;
using TriAidPlatform.API.Iam.Infrastructure.Tokens.Jwt.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace TriAidPlatform.API.Iam.Infrastructure.Tokens.Jwt.Services;

/**
 * <summary>
 *     The token service
 * </summary>
 * <remarks>
 *     This class is used to generate and validate tokens
 * </remarks>
 */
public class TokenService(IOptions<TokenSettings> tokenSettings) : ITokenService
{
    private readonly TokenSettings _tokenSettings = tokenSettings.Value;

    /**
     * <summary>
     *     Generate token
     * </summary>
     * <param name="user">The user for token generation</param>
     * <returns>The generated Token</returns>
     */
    public string GenerateToken(User user)
    {
        var secret = _tokenSettings.Secret;
        var key = Encoding.ASCII.GetBytes(secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("institution", user.Institution),
                new Claim("role", user.Role)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /**
     * <summary>
     *     Generates a 12-hour token bound to one patient. The sid claim carries the
     *     patient id and the role is always Patient, so staff policies reject it.
     * </summary>
     * <param name="session">The patient session identity</param>
     * <returns>The generated token</returns>
     */
    public string GeneratePatientToken(PatientSession session)
    {
        var key = Encoding.ASCII.GetBytes(_tokenSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Sid, session.PatientId.ToString()),
                new Claim(ClaimTypes.Name, session.Identifier),
                new Claim("patient_id", session.PatientId.ToString()),
                new Claim("role", PatientSession.Role)
            }),
            Expires = DateTime.UtcNow.AddHours(12),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
    }

    /**
     * <summary>
     *     Validates a patient portal token and returns its session, or null when the
     *     token is missing, invalid, expired or does not carry role=Patient.
     * </summary>
     * <param name="token">The bearer token</param>
     * <returns>The patient session when valid, null otherwise</returns>
     */
    public PatientSession? ValidatePatientToken(string token)
    {
        if (string.IsNullOrEmpty(token)) return null;

        var key = Encoding.ASCII.GetBytes(_tokenSettings.Secret);
        try
        {
            new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwt = (JwtSecurityToken)validatedToken;
            if (jwt.Claims.FirstOrDefault(c => c.Type == "role")?.Value != PatientSession.Role) return null;

            var patientId = int.Parse(jwt.Claims.First(c => c.Type == "patient_id").Value);
            var identifier = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? string.Empty;
            return new PatientSession(patientId, string.Empty, identifier);
        }
        catch
        {
            return null;
        }
    }

    /**
     * <summary>
     *     Validate token
     * </summary>
     * <param name="token">The token to validate</param>
     * <returns>The user id if the token is valid, null otherwise</returns>
     */
    public int? ValidateToken(string token)
    {
        if (string.IsNullOrEmpty(token)) return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_tokenSettings.Secret);
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = int.Parse(jwtToken.Claims.First(
                c => c.Type == ClaimTypes.Sid).Value);
            return userId;
        }
        catch
        {
            return null;
        }
    }
}
