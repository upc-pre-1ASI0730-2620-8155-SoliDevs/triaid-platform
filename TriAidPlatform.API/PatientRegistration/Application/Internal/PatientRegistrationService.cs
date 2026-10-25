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
public class PatientRegistrationService(IPatientRepository patientRepository, IUnitOfWork unitOfWork)
    : IPatientRegistrationService
{
    /// <inheritdoc />
    public async Task<Patient?> FindByDniAsync(string dni, CancellationToken cancellationToken = default)
    {
        return await patientRepository.GetByDniAsync(dni, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Result<Patient, PatientRegistrationError>> RegisterAsync(
        CreatePatientCommand command, CancellationToken cancellationToken = default)
    {
        if (await patientRepository.GetByDniAsync(command.Dni, cancellationToken) is not null)
            return Conflict($"A patient with DNI {command.Dni} is already registered.");

        Patient patient;
        try
        {
            patient = new Patient(command);
        }
        catch (ArgumentException e)
        {
            return Invalid(e.Message);
        }

        await patientRepository.AddAsync(patient, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<Patient, PatientRegistrationError>.Success(patient);
    }

    /// <inheritdoc />
    public async Task<Result<Patient, PatientRegistrationError>> CreateTemporaryProfileAsync(
        CreateTemporaryProfileCommand command, CancellationToken cancellationToken = default)
    {
        Patient patient;
        try
        {
            patient = new Patient(command);
        }
        catch (ArgumentException e)
        {
            return Invalid(e.Message);
        }

        await patientRepository.AddAsync(patient, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<Patient, PatientRegistrationError>.Success(patient);
    }

    /// <inheritdoc />
    public async Task<Result<Patient, PatientRegistrationError>> RegularizeIdentityAsync(
        int patientId, RegularizeIdentityCommand command, CancellationToken cancellationToken = default)
    {
        var patient = await patientRepository.FindByIdAsync(patientId, cancellationToken);
        if (patient is null)
            return NotFound(patientId);

        if (await patientRepository.GetByDniAsync(command.Dni, cancellationToken) is not null)
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

    /// <inheritdoc />
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
