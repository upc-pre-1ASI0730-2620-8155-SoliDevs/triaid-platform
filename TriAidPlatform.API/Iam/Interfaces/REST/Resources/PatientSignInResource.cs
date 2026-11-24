namespace TriAidPlatform.API.Iam.Interfaces.Rest.Resources;

/// <summary>
/// Patient portal sign-in payload. The identifier is the DNI, or the e-mail the
/// patient gave at admission. The password is the access code handed over at admission.
/// </summary>
/// <param name="Identifier">DNI or e-mail.</param>
/// <param name="Password">Plain access code.</param>
public record PatientSignInResource(string Identifier, string Password);

/// <summary>
/// Authenticated patient session returned by the portal sign-in.
/// </summary>
/// <param name="PatientId">Patient identifier.</param>
/// <param name="FullName">Display name for the portal header.</param>
/// <param name="Identifier">Identifier the patient signed in with.</param>
/// <param name="Role">Always Patient.</param>
/// <param name="Token">Bearer token scoped to this patient, valid for 12 hours.</param>
public record AuthenticatedPatientResource(int PatientId, string FullName, string Identifier, string Role, string Token);
