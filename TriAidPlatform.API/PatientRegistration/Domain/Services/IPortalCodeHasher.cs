namespace TriAidPlatform.API.PatientRegistration.Domain.Services;

/// <summary>
/// Hashes the portal access code before it is stored. The plain code is only
/// shown once at the admission desk.
/// </summary>
public interface IPortalCodeHasher
{
    /// <summary>
    /// Hashes a plain access code.
    /// </summary>
    /// <param name="code">Plain access code.</param>
    /// <returns>Irreversible hash.</returns>
    string Hash(string code);
}
