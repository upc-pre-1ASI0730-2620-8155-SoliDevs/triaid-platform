namespace TriAidPlatform.API.PatientRegistration.Interfaces.REST.Resources;

/// <summary>Payload to enable the patient portal from the admission desk.</summary>
/// <param name="Email">Optional contact e-mail; also accepted to sign in.</param>
public record EnablePortalResource(string? Email);

/// <summary>
/// Result of enabling the portal. The access code is shown once so the admission
/// desk can hand it to the patient; it is never returned again.
/// </summary>
/// <param name="PatientId">Patient identifier.</param>
/// <param name="Email">Stored e-mail, if any.</param>
/// <param name="AccessCode">Plain access code to hand over.</param>
public record PortalEnabledResource(int PatientId, string? Email, string AccessCode);
