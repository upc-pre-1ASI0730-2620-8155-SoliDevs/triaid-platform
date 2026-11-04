namespace TriAidPlatform.API.PatientRegistration.Domain.Model.Commands;

/// <summary>
/// Command to assign the official identity document to a temporary patient
/// (US07, scenario 2).
/// </summary>
/// <param name="Dni">Valid national identity document number (8 digits).</param>
public record RegularizeIdentityCommand(string Dni);
