using TriAidPlatform.API.PatientRegistration.Domain.Services;

namespace TriAidPlatform.API.PatientRegistration.Infrastructure.Hashing;

/// <summary>
/// BCrypt implementation of the portal code hasher. Reuses the same algorithm
/// IAM uses for staff passwords so the IAM verifier accepts these hashes.
/// </summary>
public class BCryptPortalCodeHasher : IPortalCodeHasher
{
    /// <inheritdoc />
    public string Hash(string code) => BCrypt.Net.BCrypt.HashPassword(code);
}
