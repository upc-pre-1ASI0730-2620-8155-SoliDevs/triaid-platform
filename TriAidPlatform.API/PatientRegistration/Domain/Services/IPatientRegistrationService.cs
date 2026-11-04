using TriAidPlatform.API.PatientRegistration.Domain.Model.Aggregates;
using TriAidPlatform.API.PatientRegistration.Domain.Model.Commands;
using TriAidPlatform.API.PatientRegistration.Domain.Model.ValueObjects;
using TriAidPlatform.API.Shared.Application.Patterns;

namespace TriAidPlatform.API.PatientRegistration.Domain.Services;

/// <summary>
/// Use cases of the Patient Registration bounded context: search by DNI,
/// registration, emergency temporary profiles and identity regularization.
/// </summary>
public interface IPatientRegistrationService
{
    /// <summary>
    /// Searches a patient by DNI (US06).
    /// </summary>
    /// <param name="dni">The DNI to search.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The <see cref="Patient"/> if found; otherwise, null.</returns>
    Task<Patient?> FindByDniAsync(string dni, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a patient with identity document (US06, US08).
    /// </summary>
    /// <param name="command">The registration data.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The registered patient or the failure.</returns>
    Task<Result<Patient, PatientRegistrationError>> RegisterAsync(
        CreatePatientCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an emergency temporary profile for a patient without document (US07).
    /// </summary>
    /// <param name="command">The basic patient data.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The temporary patient or the failure.</returns>
    Task<Result<Patient, PatientRegistrationError>> CreateTemporaryProfileAsync(
        CreateTemporaryProfileCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns the official DNI to a temporary patient (US07, scenario 2).
    /// </summary>
    /// <param name="patientId">Identifier of the temporary patient.</param>
    /// <param name="command">The official DNI.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The regularized patient or the failure.</returns>
    Task<Result<Patient, PatientRegistrationError>> RegularizeIdentityAsync(
        int patientId, RegularizeIdentityCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the contact data of a patient.
    /// </summary>
    /// <param name="patientId">Identifier of the patient.</param>
    /// <param name="command">The new contact data.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The updated patient or the failure.</returns>
    Task<Result<Patient, PatientRegistrationError>> UpdateContactDataAsync(
        int patientId, UpdateContactCommand command, CancellationToken cancellationToken = default);
}
