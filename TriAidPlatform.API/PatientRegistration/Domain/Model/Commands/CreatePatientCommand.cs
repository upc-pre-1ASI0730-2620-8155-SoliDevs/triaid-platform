using TriAidPlatform.API.PatientRegistration.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.PatientRegistration.Domain.Model.Commands;

/// <summary>
/// Command to register a patient that presents a valid national identity document (US06).
/// </summary>
/// <param name="Dni">National identity document number (8 digits).</param>
/// <param name="FirstName">Patient first name.</param>
/// <param name="LastName">Patient last name.</param>
/// <param name="BirthDate">Patient birth date (required, US08).</param>
/// <param name="Sex">Patient biological sex.</param>
/// <param name="Phone">Optional contact phone.</param>
/// <param name="Address">Optional home address.</param>
public record CreatePatientCommand(
    string Dni,
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    Sex Sex,
    string? Phone,
    string? Address);
