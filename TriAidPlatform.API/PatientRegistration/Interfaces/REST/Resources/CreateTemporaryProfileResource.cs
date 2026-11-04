namespace TriAidPlatform.API.PatientRegistration.Interfaces.REST.Resources;

/// <summary>Payload to create an emergency temporary profile.</summary>
/// <param name="FirstName">Patient first name (required).</param>
/// <param name="LastName">Patient last name (required).</param>
/// <param name="BirthDate">Patient birth date (required, yyyy-MM-dd).</param>
/// <param name="Sex">Patient sex: Male or Female (required).</param>
public record CreateTemporaryProfileResource(
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    string Sex);
