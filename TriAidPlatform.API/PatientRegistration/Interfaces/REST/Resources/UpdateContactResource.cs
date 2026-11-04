namespace TriAidPlatform.API.PatientRegistration.Interfaces.REST.Resources;

/// <summary>Payload to update the contact data of a patient.</summary>
/// <param name="Phone">New contact phone (optional).</param>
/// <param name="Address">New home address (optional).</param>
public record UpdateContactResource(string? Phone, string? Address);
