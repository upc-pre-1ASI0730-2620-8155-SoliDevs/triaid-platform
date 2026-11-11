using TriAidPlatform.API.Iam.Domain.Services;

namespace TriAidPlatform.API.Iam.Infrastructure.Hashing.BCrypt.Services;

/// <summary>BCrypt implementation of the password hashing port.</summary>
public class BCryptHashingService : IPasswordHasher
{
    /// <inheritdoc />
    public string HashPassword(string password) =>
        global::BCrypt.Net.BCrypt.HashPassword(password);

    /// <inheritdoc />
    public bool VerifyPassword(string password, string passwordHash) =>
        global::BCrypt.Net.BCrypt.Verify(password, passwordHash);
}
