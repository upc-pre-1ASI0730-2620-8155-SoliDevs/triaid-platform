namespace TriAidPlatform.API.PatientRegistration.Interfaces.REST.Resources;

/// <summary>
/// Patient just registered. The portal access code is included once so the admission
/// desk can hand it to the patient; it is never returned again by any other endpoint.
/// </summary>
/// <param name="Patient">The registered patient.</param>
/// <param name="PortalAccessCode">One-time portal access code.</param>
public record PatientCreatedResource(PatientResource Patient, string PortalAccessCode);
