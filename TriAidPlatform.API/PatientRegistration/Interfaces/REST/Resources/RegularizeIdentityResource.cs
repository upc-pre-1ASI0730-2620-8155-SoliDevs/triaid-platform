namespace TriAidPlatform.API.PatientRegistration.Interfaces.REST.Resources;

/// <summary>Payload to regularize the identity of a temporary patient.</summary>
/// <param name="Dni">Official national identity document number (8 digits).</param>
public record RegularizeIdentityResource(string Dni);
