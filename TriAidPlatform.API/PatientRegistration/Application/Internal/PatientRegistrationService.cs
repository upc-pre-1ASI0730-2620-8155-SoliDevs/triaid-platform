using TriAidPlatform.API.PatientRegistration.Domain.Model.Aggregates;
using TriAidPlatform.API.PatientRegistration.Domain.Model.Commands;
using TriAidPlatform.API.PatientRegistration.Domain.Model.ValueObjects;
using TriAidPlatform.API.PatientRegistration.Domain.Repositories;
using TriAidPlatform.API.PatientRegistration.Domain.Services;
using TriAidPlatform.API.Shared.Application.Patterns;
using TriAidPlatform.API.Shared.Domain.Repositories;

namespace TriAidPlatform.API.PatientRegistration.Application.Internal;

/// <summary>
/// Application service that orchestrates the patient registration use cases.
/// Business rules live in the <see cref="Patient"/> aggregate; this service
/// checks uniqueness and persists the changes through the unit of work.
/// </summary>
/// <param name="patientRepository">Repository of the Patient aggregate.</param>
/// <param name="unitOfWork">Unit of work used to commit the changes.</param>
public class PatientRegistrationService(
    IPatientRepository patientRepository,
    IUnitOfWork unitOfWork,
    IPortalCodeHasher portalCodeHasher) : IPatientRegistrationService
{
    /// <summary>
    /// Searches a patient by its national identity document number (US06).
    /// </summary>
    /// <param name="dni">The DNI to search.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>The <see cref="Patient"/> if found; otherwise, null.</returns>
    public async Task<Patient?> FindByDniAsync(string dni, string institution, CancellationToken cancellationToken = default)
    {
        return await patientRepository.GetByDniAsync(dni, institution, cancellationToken);
    }

    /// <summary>
    /// Registers a patient with identity document. Rejects the operation when the DNI
    /// is already registered or when the patient data breaks a business rule (US06, US08).
    /// </summary>
    /// <param name="command">The registration data.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>The registered patient, or a Conflict or Validation failure.</returns>
    public async Task<Result<(Patient Patient, string AccessCode), PatientRegistrationError>> RegisterAsync(
        CreatePatientCommand command, CancellationToken cancellationToken = default)
    {
        if (await patientRepository.GetByDniAsync(command.Dni, command.Institution ?? string.Empty, cancellationToken) is not null)
            return new Result<(Patient, string), PatientRegistrationError>.Failure(
                new PatientRegistrationError(EPatientRegistrationErrorType.Conflict,
                    $"A patient with DNI {command.Dni} is already registered."));

        Patient patient;
        try
        {
            patient = new Patient(command);
            patient.AssignInstitution(command.Institution);
        }
        catch (ArgumentException e)
        {
            return new Result<(Patient, string), PatientRegistrationError>.Failure(
                new PatientRegistrationError(EPatientRegistrationErrorType.Validation, e.Message));
        }

        // Every registered patient gets portal access right away: the plan includes
        // the portal for all patients, and admission hands the code over in the same step.
        var code = GeneratePortalCode();
        patient.EnablePortal(command.Email);
        patient.SetPortalPasswordHash(portalCodeHasher.Hash(code));

        await patientRepository.AddAsync(patient, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<(Patient, string), PatientRegistrationError>.Success((patient, code));
    }

    /// <summary>
    /// Creates an emergency temporary profile for a patient without identity document
    /// and assigns a unique temporary code (US07).
    /// </summary>
    /// <param name="command">The basic patient data.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>The temporary patient, or a Validation failure.</returns>
    public async Task<Result<Patient, PatientRegistrationError>> CreateTemporaryProfileAsync(
        CreateTemporaryProfileCommand command, CancellationToken cancellationToken = default)
    {
        Patient patient;
        try
        {
            patient = new Patient(command);
            patient.AssignInstitution(command.Institution);
        }
        catch (ArgumentException e)
        {
            return Invalid(e.Message);
        }

        await patientRepository.AddAsync(patient, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<Patient, PatientRegistrationError>.Success(patient);
    }

    /// <summary>
    /// Assigns the official DNI to a temporary patient, keeping the same record so the
    /// temporary history stays linked to the official identity (US07, scenario 2).
    /// </summary>
    /// <param name="patientId">Identifier of the temporary patient.</param>
    /// <param name="command">The official DNI.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>The regularized patient, or a NotFound, Conflict or Validation failure.</returns>
    public async Task<Result<Patient, PatientRegistrationError>> RegularizeIdentityAsync(
        int patientId, RegularizeIdentityCommand command, CancellationToken cancellationToken = default)
    {
        var patient = await patientRepository.FindByIdAsync(patientId, cancellationToken);
        if (patient is null)
            return NotFound(patientId);

        if (await patientRepository.GetByDniAsync(command.Dni, patient.Institution, cancellationToken) is not null)
            return Conflict($"A patient with DNI {command.Dni} is already registered.");

        try
        {
            patient.RegularizeIdentity(command.Dni);
        }
        catch (Exception e) when (e is ArgumentException or InvalidOperationException)
        {
            return Invalid(e.Message);
        }

        patientRepository.Update(patient);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<Patient, PatientRegistrationError>.Success(patient);
    }

    /// <summary>
    /// Updates the contact phone and address of a registered patient.
    /// </summary>
    /// <param name="patientId">Identifier of the patient.</param>
    /// <param name="command">The new contact data.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>The updated patient, or a NotFound failure.</returns>
    public async Task<Result<Patient, PatientRegistrationError>> UpdateContactDataAsync(
        int patientId, UpdateContactCommand command, CancellationToken cancellationToken = default)
    {
        var patient = await patientRepository.FindByIdAsync(patientId, cancellationToken);
        if (patient is null)
            return NotFound(patientId);

        patient.UpdateContactData(command.Phone, command.Address);
        patientRepository.Update(patient);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<Patient, PatientRegistrationError>.Success(patient);
    }

    /// <summary>Builds an 8-character access code made of unambiguous characters.</summary>
    private static string GeneratePortalCode()
    {
        const string alphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(8);
        return new string(bytes.Select(b => alphabet[b % alphabet.Length]).ToArray());
    }

    /// <summary>
    /// Generates a fresh 8-character access code, stores its hash and optional
    /// e-mail, and returns the plain code for the admission desk. Calling it again
    /// rotates the code, which is how a forgotten code is replaced.
    /// </summary>
    public async Task<Result<(Patient Patient, string AccessCode), PatientRegistrationError>> EnablePortalAsync(
        int patientId, EnablePatientPortalCommand command, CancellationToken cancellationToken = default)
    {
        var patient = await patientRepository.FindByIdAsync(patientId, cancellationToken);
        if (patient is null)
            return new Result<(Patient, string), PatientRegistrationError>.Failure(
                new PatientRegistrationError(EPatientRegistrationErrorType.NotFound,
                    $"Patient with id {patientId} was not found."));

        var code = GeneratePortalCode();

        patient.EnablePortal(command.Email);
        patient.SetPortalPasswordHash(portalCodeHasher.Hash(code));
        patientRepository.Update(patient);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<(Patient, string), PatientRegistrationError>.Success((patient, code));
    }

    private static Result<Patient, PatientRegistrationError> NotFound(int patientId) =>
        new Result<Patient, PatientRegistrationError>.Failure(
            new PatientRegistrationError(EPatientRegistrationErrorType.NotFound,
                $"Patient with id {patientId} was not found."));

    private static Result<Patient, PatientRegistrationError> Conflict(string message) =>
        new Result<Patient, PatientRegistrationError>.Failure(
            new PatientRegistrationError(EPatientRegistrationErrorType.Conflict, message));

    private static Result<Patient, PatientRegistrationError> Invalid(string message) =>
        new Result<Patient, PatientRegistrationError>.Failure(
            new PatientRegistrationError(EPatientRegistrationErrorType.Validation, message));
}
