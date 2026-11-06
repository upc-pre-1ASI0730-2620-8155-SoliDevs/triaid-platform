using Microsoft.AspNetCore.Mvc;
using TriAidPlatform.API.PatientRegistration.Domain.Model.Aggregates;
using TriAidPlatform.API.PatientRegistration.Domain.Model.ValueObjects;
using TriAidPlatform.API.PatientRegistration.Domain.Repositories;
using TriAidPlatform.API.PatientRegistration.Domain.Services;
using TriAidPlatform.API.PatientRegistration.Interfaces.REST.Resources;
using TriAidPlatform.API.PatientRegistration.Interfaces.REST.Transform;
using TriAidPlatform.API.Shared.Application.Patterns;

namespace TriAidPlatform.API.PatientRegistration.Interfaces.REST;

/// <summary>
/// REST facade of the Patient Registration bounded context (US06, US07, US08).
/// Exposes patient search, registration, temporary profiles and identity regularization under /patients.
/// </summary>
/// <param name="patientRegistrationService">Use cases of the bounded context.</param>
/// <param name="patientRepository">Repository used for read-only listings.</param>
[ApiController]
[Route("patients")]
[Produces("application/json")]
public class PatientsController(
    IPatientRegistrationService patientRegistrationService,
    IPatientRepository patientRepository) : ControllerBase
{
    /// <summary>Lists every registered patient.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PatientResource>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var patients = await patientRepository.ListAsync(cancellationToken);
        return Ok(patients.Select(PatientResourceFromEntityAssembler.ToResourceFromEntity));
    }

    /// <summary>Gets a single patient by its identifier.</summary>
    [HttpGet("{patientId:int}")]
    public async Task<ActionResult<PatientResource>> GetById(int patientId, CancellationToken cancellationToken)
    {
        var patient = await patientRepository.FindByIdAsync(patientId, cancellationToken);
        return patient is null
            ? NotFound(new { message = $"Patient with id {patientId} was not found." })
            : Ok(PatientResourceFromEntityAssembler.ToResourceFromEntity(patient));
    }

    /// <summary>Searches a patient by DNI to autocomplete the admission form (US06).</summary>
    [HttpGet("dni/{dni}")]
    public async Task<ActionResult<PatientResource>> GetByDniAsync(string dni, CancellationToken cancellationToken)
    {
        var patient = await patientRegistrationService.FindByDniAsync(dni, cancellationToken);
        return patient is null
            ? NotFound(new { message = $"No patient registered with DNI {dni}." })
            : Ok(PatientResourceFromEntityAssembler.ToResourceFromEntity(patient));
    }

    /// <summary>Registers a patient with identity document (US06, US08).</summary>
    [HttpPost]
    public async Task<ActionResult<PatientResource>> CreateAsync(
        [FromBody] CreatePatientResource resource, CancellationToken cancellationToken)
    {
        try
        {
            var command = PatientCommandFromResourceAssembler.ToCommandFromResource(resource);
            var result = await patientRegistrationService.RegisterAsync(command, cancellationToken);
            return ToCreatedResponse(result);
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    /// <summary>Creates an emergency temporary profile for a patient without document (US07).</summary>
    [HttpPost("temporary")]
    public async Task<ActionResult<PatientResource>> CreateTemporaryProfileAsync(
        [FromBody] CreateTemporaryProfileResource resource, CancellationToken cancellationToken)
    {
        try
        {
            var command = PatientCommandFromResourceAssembler.ToCommandFromResource(resource);
            var result = await patientRegistrationService.CreateTemporaryProfileAsync(command, cancellationToken);
            return ToCreatedResponse(result);
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    /// <summary>Assigns the official DNI to a temporary patient (US07, scenario 2).</summary>
    [HttpPut("{patientId:int}/identity")]
    public async Task<ActionResult<PatientResource>> RegularizeIdentityAsync(
        int patientId, [FromBody] RegularizeIdentityResource resource, CancellationToken cancellationToken)
    {
        var command = PatientCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await patientRegistrationService.RegularizeIdentityAsync(patientId, command, cancellationToken);
        return ToOkResponse(result);
    }

    /// <summary>Updates the contact data of a patient.</summary>
    [HttpPut("{patientId:int}/contact")]
    public async Task<ActionResult<PatientResource>> UpdateContactDataAsync(
        int patientId, [FromBody] UpdateContactResource resource, CancellationToken cancellationToken)
    {
        var command = PatientCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await patientRegistrationService.UpdateContactDataAsync(patientId, command, cancellationToken);
        return ToOkResponse(result);
    }

    private ActionResult<PatientResource> ToCreatedResponse(Result<Patient, PatientRegistrationError> result) =>
        result.Fold<ActionResult<PatientResource>>(
            patient =>
            {
                var created = PatientResourceFromEntityAssembler.ToResourceFromEntity(patient);
                return CreatedAtAction(nameof(GetById), new { patientId = created.Id }, created);
            },
            ToErrorResponse);

    private ActionResult<PatientResource> ToOkResponse(Result<Patient, PatientRegistrationError> result) =>
        result.Fold<ActionResult<PatientResource>>(
            patient => Ok(PatientResourceFromEntityAssembler.ToResourceFromEntity(patient)),
            ToErrorResponse);

    private ActionResult<PatientResource> ToErrorResponse(PatientRegistrationError error) => error.Type switch
    {
        EPatientRegistrationErrorType.NotFound => NotFound(new { message = error.Message }),
        EPatientRegistrationErrorType.Conflict => Conflict(new { message = error.Message }),
        _ => BadRequest(new { message = error.Message })
    };
}
