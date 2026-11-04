namespace TriAidPlatform.API.PatientRegistration.Domain.Model.Commands;

/// <summary>
/// Command to update the contact data of a registered patient.
/// </summary>
/// <param name="Phone">New contact phone.</param>
/// <param name="Address">New home address.</param>
public record UpdateContactCommand(string? Phone, string? Address);
