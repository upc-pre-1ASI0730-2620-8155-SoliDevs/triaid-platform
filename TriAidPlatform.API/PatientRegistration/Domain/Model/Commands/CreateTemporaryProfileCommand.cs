using TriAidPlatform.API.PatientRegistration.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.PatientRegistration.Domain.Model.Commands;

/// <summary>
/// Command to create an emergency temporary profile for a patient without
/// identity document (US07, scenario 1).
/// </summary>
/// <param name="FirstName">Patient first name.</param>
/// <param name="LastName">Patient last name.</param>
/// <param name="BirthDate">Patient birth date (required, US08).</param>
/// <param name="Sex">Patient biological sex.</param>
public record CreateTemporaryProfileCommand(
    string FirstName,
    string LastName,
    DateOnly BirthDate,
    Sex Sex);
