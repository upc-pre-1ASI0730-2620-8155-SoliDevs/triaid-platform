namespace TriAidPlatform.API.Iam.Application.Internal.OutboundServices;

/// <summary>
/// Read-only view of the Patient Registration bounded context used by IAM to
/// authenticate patients at the portal. Keeps IAM free of patient persistence.
/// </summary>
public interface IPatientDirectory
{
    /// <summary>
    /// Finds a patient by DNI or contact e-mail.
    /// </summary>
    /// <param name="identifier">Identifier typed at the portal sign-in.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<PatientCredentials?> FindAsync(string identifier, CancellationToken cancellationToken = default);
}

/// <summary>
/// Minimum patient identity IAM needs to issue a portal token.
/// </summary>
/// <param name="PatientId">Patient aggregate identifier.</param>
/// <param name="FullName">Display name for the portal header.</param>
/// <param name="Identifier">The identifier the patient signed in with.</param>
/// <param name="PasswordHash">BCrypt hash of the portal access code, or null when the portal is not enabled.</param>
public record PatientCredentials(int PatientId, string FullName, string Identifier, string? PasswordHash);
