namespace TriAidPlatform.API.PatientRegistration.Interfaces.REST.Resources;

/// <summary>Payload to register a patient with identity document.</summary>
/// <param name="Dni">National identity document number (required, 8 digits).</param>
/// <param name="FirstName">Patient first name (required).</param>
/// <param name="LastName">Patient last name (required).</param>
/// <param name="BirthDate">Patient birth date (required, yyyy-MM-dd).</param>
/// <param name="Sex">Patient sex: Male or Female (required).</param>
/// <param name="Phone">Contact phone (optional).</param>
/// <param name="Address">Home address (optional).</param>
public record CreatePatientResource(
    string Dni,
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    string Sex,
    string? Phone,
    string? Address);
