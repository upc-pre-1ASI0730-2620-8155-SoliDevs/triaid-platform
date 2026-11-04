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
/// REST facade of the triage episode that starts the emergency attention of a patient.
/// Exposes the episode lifecycle under /triage-episodes.
/// </summary>
/// <param name="triageEpisodeService">Use cases of the triage episode.</param>
/// <param name="episodeRepository">Repository used for read-only queries.</param>
[ApiController]
[Route("triage-episodes")]
[Produces("application/json")]
public class TriageEpisodesController(
    ITriageEpisodeService triageEpisodeService,
    ITriageEpisodeRepository episodeRepository) : ControllerBase
{
    /// <summary>Lists every triage episode.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TriageEpisodeResource>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var episodes = await episodeRepository.ListAsync(cancellationToken);
        return Ok(episodes.Select(TriageEpisodeResourceFromEntityAssembler.ToResourceFromEntity));
    }

    /// <summary>Gets a single triage episode by its identifier.</summary>
    [HttpGet("{episodeId:int}")]
    public async Task<ActionResult<TriageEpisodeResource>> GetById(int episodeId, CancellationToken cancellationToken)
    {
        var episode = await episodeRepository.FindByIdAsync(episodeId, cancellationToken);
        return episode is null
            ? NotFound(new { message = $"Triage episode with id {episodeId} was not found." })
            : Ok(TriageEpisodeResourceFromEntityAssembler.ToResourceFromEntity(episode));
    }

    /// <summary>Lists the episodes of a patient, most recent first.</summary>
    [HttpGet("patient/{patientId:int}")]
    public async Task<ActionResult<IEnumerable<TriageEpisodeResource>>> GetByPatientIdAsync(
        int patientId, CancellationToken cancellationToken)
    {
        var episodes = await episodeRepository.FindByPatientIdAsync(patientId, cancellationToken);
        return Ok(episodes.Select(TriageEpisodeResourceFromEntityAssembler.ToResourceFromEntity));
    }

    /// <summary>Opens a new triage episode for a registered patient.</summary>
    [HttpPost]
    public async Task<ActionResult<TriageEpisodeResource>> OpenAsync(
        [FromBody] OpenTriageEpisodeResource resource, CancellationToken cancellationToken)
    {
        try
        {
            var command = TriageEpisodeResourceFromEntityAssembler.ToCommandFromResource(resource);
            var result = await triageEpisodeService.OpenAsync(command, cancellationToken);
            return result.Fold<ActionResult<TriageEpisodeResource>>(
                episode =>
                {
                    var created = TriageEpisodeResourceFromEntityAssembler.ToResourceFromEntity(episode);
                    return CreatedAtAction(nameof(GetById), new { episodeId = created.Id }, created);
                },
                ToErrorResponse);
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    /// <summary>Moves a waiting episode to triage.</summary>
    [HttpPut("{episodeId:int}/start-triage")]
    public async Task<ActionResult<TriageEpisodeResource>> StartTriageAsync(
        int episodeId, CancellationToken cancellationToken) =>
        ToOkResponse(await triageEpisodeService.StartTriageAsync(episodeId, cancellationToken));

    /// <summary>Closes an episode in triage as attended.</summary>
    [HttpPut("{episodeId:int}/attended")]
    public async Task<ActionResult<TriageEpisodeResource>> MarkAttendedAsync(
        int episodeId, CancellationToken cancellationToken) =>
        ToOkResponse(await triageEpisodeService.MarkAttendedAsync(episodeId, cancellationToken));

    /// <summary>Closes an episode in triage as referred to a specialty.</summary>
    [HttpPut("{episodeId:int}/referred")]
    public async Task<ActionResult<TriageEpisodeResource>> MarkReferredAsync(
        int episodeId, CancellationToken cancellationToken) =>
        ToOkResponse(await triageEpisodeService.MarkReferredAsync(episodeId, cancellationToken));

    private ActionResult<TriageEpisodeResource> ToOkResponse(Result<TriageEpisode, PatientRegistrationError> result) =>
        result.Fold<ActionResult<TriageEpisodeResource>>(
            episode => Ok(TriageEpisodeResourceFromEntityAssembler.ToResourceFromEntity(episode)),
            ToErrorResponse);

    private ActionResult<TriageEpisodeResource> ToErrorResponse(PatientRegistrationError error) => error.Type switch
    {
        EPatientRegistrationErrorType.NotFound => NotFound(new { message = error.Message }),
        EPatientRegistrationErrorType.Conflict => Conflict(new { message = error.Message }),
        _ => BadRequest(new { message = error.Message })
    };
}
