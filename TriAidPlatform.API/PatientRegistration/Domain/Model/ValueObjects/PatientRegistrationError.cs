namespace TriAidPlatform.API.PatientRegistration.Domain.Model.ValueObjects;

/// <summary>
/// Kind of failure returned by the patient registration use cases.
/// </summary>
public enum EPatientRegistrationErrorType
{
    /// <summary>The requested patient does not exist.</summary>
    NotFound,

    /// <summary>The operation collides with an existing patient.</summary>
    Conflict,

    /// <summary>The submitted data breaks a business rule.</summary>
    Validation
}

/// <summary>
/// Failure returned by the patient registration use cases through the Result pattern.
/// </summary>
/// <param name="Type">Kind of failure.</param>
/// <param name="Message">Human-readable description of the failure.</param>
public record PatientRegistrationError(EPatientRegistrationErrorType Type, string Message);
