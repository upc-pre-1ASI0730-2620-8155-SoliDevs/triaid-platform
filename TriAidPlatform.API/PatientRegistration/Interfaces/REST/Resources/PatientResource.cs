namespace TriAidPlatform.API.PatientRegistration.Interfaces.REST.Resources;

/// <summary>Patient data returned by the API.</summary>
/// <param name="Id">Patient identifier.</param>
/// <param name="Dni">National identity document number, null for temporary profiles.</param>
/// <param name="FirstName">Patient first name.</param>
/// <param name="LastName">Patient last name.</param>
/// <param name="BirthDate">Patient birth date (yyyy-MM-dd).</param>
/// <param name="Sex">Patient sex (Male or Female).</param>
/// <param name="Phone">Contact phone.</param>
/// <param name="Address">Home address.</param>
/// <param name="IsTemporary">Whether the profile is an emergency temporary profile.</param>
/// <param name="TemporaryCode">Emergency identifier of temporary profiles.</param>
public record PatientResource(
    int Id,
    string? Dni,
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    string Sex,
    string? Phone,
    string? Address,
    bool IsTemporary,
    string? TemporaryCode);
