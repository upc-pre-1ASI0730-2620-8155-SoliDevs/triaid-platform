using Microsoft.AspNetCore.Mvc;
using TriAidPlatform.API.Shared.Application.Patterns;
using TriAidPlatform.API.SpecialtyAssignment.Application.Internal.CommandServices;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Aggregates;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Commands;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Queries;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Repositories;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Services;
using TriAidPlatform.API.Shared.Domain.Repositories;
using TriAidPlatform.API.SpecialtyAssignment.Interfaces.REST.Resources;
using TriAidPlatform.API.SpecialtyAssignment.Interfaces.REST.Transform;

namespace TriAidPlatform.API.SpecialtyAssignment.Interfaces.REST;

/// <summary>
/// REST facade of the Specialty Assignment bounded context
/// (US30-US33, US37). Exposes the specialty catalog, the rule-based
/// suggestion, the referral with queue position and the justified
/// reassignment, plus the full-referral CRUD used by the web client
/// (ADR-010 backend swap contract).
/// </summary>
/// <param name="assignmentService">Use cases of the bounded context.</param>
/// <param name="referralRepository">Repository of the Referral aggregate.</param>
/// <param name="specialtyRepository">Repository of the Specialty catalog.</param>
/// <param name="unitOfWork">Unit of work used to commit the changes.</param>
[ApiController]
[Route("specialty-assignment")]
[Produces("application/json")]
public class SpecialtyAssignmentController(
    ISpecialtyAssignmentService assignmentService,
    IReferralRepository referralRepository,
    ISpecialtyRepository specialtyRepository,
    IUnitOfWork unitOfWork) : ControllerBase
{
    /// <summary>Lists the specialty catalog.</summary>
    /// <param name="onlyActive">When true, only active specialties are returned.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>The specialty catalog.</returns>
    [HttpGet("specialties")]
    public async Task<ActionResult<IEnumerable<SpecialtyResource>>> GetSpecialties(
        [FromQuery] bool onlyActive = false, CancellationToken cancellationToken = default)
    {
        var specialties = await assignmentService.ListSpecialtiesAsync(onlyActive, cancellationToken);
        return Ok(specialties.Select(SpecialtyResource.FromEntity));
    }

    /// <summary>Computes the suggested specialty for an episode (US30).</summary>
    /// <param name="resource">Symptom and patient context of the episode.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>The suggested specialty with the strategy that produced it.</returns>
    [HttpPost("suggest")]
    public async Task<ActionResult<SpecialtySuggestionResult>> Suggest(
        [FromBody] SuggestSpecialtyResource resource, CancellationToken cancellationToken)
    {
        var query = new SuggestSpecialtyQuery(
            string.Empty, resource.Symptom, resource.TriageLevel, resource.PatientAge, resource.PatientSex);
        var result = await assignmentService.SuggestAsync(query, cancellationToken);
        return result.Fold<ActionResult<SpecialtySuggestionResult>>(
            suggestion => Ok(suggestion),
            error => error.Type switch
            {
                ESpecialtyError.Validation => BadRequest(new { message = error.Message }),
                _ => NotFound(new { message = error.Message })
            });
    }

    /// <summary>Routes a classified episode to a specialty (US31).</summary>
    /// <param name="episodeId">Identifier of the classified episode.</param>
    /// <param name="resource">Destination specialty and symptom.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>The created referral with its queue position.</returns>
    [HttpPost("episode/{episodeId}/refer")]
    public async Task<ActionResult<ReferralResource>> Refer(string episodeId,
        [FromBody] ReferEpisodeResource resource, CancellationToken cancellationToken)
    {
        var command = new ReferEpisodeCommand(episodeId, resource.SpecialtyId);
        var result = await assignmentService.ReferAsync(command, resource.Symptom, cancellationToken);
        return await ToReferralResultAsync(result, cancellationToken);
    }

    /// <summary>Reassigns a referral with a written justification (US32).</summary>
    /// <param name="referralId">Identifier of the referral.</param>
    /// <param name="resource">New specialty and justification.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>The reassigned referral.</returns>
    [HttpPost("referrals/{referralId:int}/reassign")]
    public async Task<ActionResult<ReferralResource>> Reassign(int referralId,
        [FromBody] ReassignReferralResource resource, CancellationToken cancellationToken)
    {
        var command = new ReassignReferralCommand(referralId, resource.NewSpecialtyId, resource.Reason);
        var result = await assignmentService.ReassignAsync(command, cancellationToken);
        return await ToReferralResultAsync(result, cancellationToken);
    }

    /// <summary>Retrieves the referral of an episode with its queue position (US33, US37).</summary>
    /// <param name="episodeId">Identifier of the episode.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>The referral, or 404 when the episode is not referred.</returns>
    [HttpGet("episode/{episodeId}/referral")]
    public async Task<ActionResult<ReferralResource>> GetReferral(string episodeId,
        CancellationToken cancellationToken)
    {
        var result = await assignmentService.GetByEpisodeAsync(episodeId, cancellationToken);
        return await ToReferralResultAsync(result, cancellationToken);
    }

    /**
     * <summary>
     *     Lists every referral across institutions. The web client
     *     filters by institution client-side (ADR-010)
     * </summary>
     * <param name="cancellationToken">Token to observe for cancellation.</param>
     * <returns>All referrals with their specialty names.</returns>
     */
    [HttpGet("referrals")]
    public async Task<ActionResult<IEnumerable<ReferralResource>>> GetReferrals(
        CancellationToken cancellationToken)
    {
        var referrals = await referralRepository.ListAsync(cancellationToken);
        var names = await SpecialtyNamesAsync(cancellationToken);
        return Ok(referrals.Select(r => ReferralResourceFromEntityAssembler.ToResourceFromEntity(
            r, names.GetValueOrDefault(r.SpecialtyId, r.SpecialtyId.ToString()))));
    }

    /**
     * <summary>
     *     Creates a referral from the web client record. The specialty
     *     arrives by catalog name and the client reports the queue
     *     position and timestamps (ADR-010)
     * </summary>
     * <param name="resource">The referral record sent by the client.</param>
     * <param name="cancellationToken">Token to observe for cancellation.</param>
     * <returns>The created referral.</returns>
     * <response code="200">Referral created.</response>
     * <response code="400">Unknown specialty or missing institution.</response>
     */
    [HttpPost("referrals")]
    public async Task<ActionResult<ReferralResource>> CreateReferral(
        [FromBody] ReferralResource resource, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(resource.Institution))
            return BadRequest(new { message = "The institution is required." });
        var specialty = await FindByNameAsync(resource.Specialty, cancellationToken);
        if (specialty is null)
            return BadRequest(new { message = $"Specialty {resource.Specialty} is not part of the catalog." });

        var referral = new Referral(
                new ReferEpisodeCommand(resource.EpisodeId, specialty.Id), specialty.Room,
                resource.QueuePosition)
            .AttachContext(resource.Symptom, resource.Institution, resource.State, resource.ReferredAt);
        await referralRepository.AddAsync(referral, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return Ok(ReferralResourceFromEntityAssembler.ToResourceFromEntity(referral, specialty.Name));
    }

    /**
     * <summary>
     *     Applies a full-record referral update from the web client
     *     (ADR-010)
     * </summary>
     * <param name="id">Identifier of the referral.</param>
     * <param name="resource">The referral record sent by the client.</param>
     * <param name="cancellationToken">Token to observe for cancellation.</param>
     * <returns>The updated referral.</returns>
     * <response code="200">Referral updated.</response>
     * <response code="400">Unknown specialty.</response>
     * <response code="404">Referral not found.</response>
     */
    [HttpPut("referrals/{id:int}")]
    public async Task<ActionResult<ReferralResource>> UpdateReferral(int id,
        [FromBody] ReferralResource resource, CancellationToken cancellationToken)
    {
        var referral = (await referralRepository.ListAsync(cancellationToken)).FirstOrDefault(r => r.Id == id);
        if (referral is null)
            return NotFound(new { message = $"Referral with id {id} was not found." });
        var specialty = await FindByNameAsync(resource.Specialty, cancellationToken);
        if (specialty is null)
            return BadRequest(new { message = $"Specialty {resource.Specialty} is not part of the catalog." });

        referral.ApplyRecordUpdate(specialty.Id, specialty.Room, resource.QueuePosition,
            resource.State, resource.ChangeReason);
        await unitOfWork.CompleteAsync(cancellationToken);
        return Ok(ReferralResourceFromEntityAssembler.ToResourceFromEntity(referral, specialty.Name));
    }

    private async Task<ActionResult<ReferralResource>> ToReferralResultAsync(
        Result<Referral, SpecialtyError> result, CancellationToken cancellationToken)
    {
        if (result is Result<Referral, SpecialtyError>.Failure failure)
            return ToErrorResponse(failure.Error);
        var referral = ((Result<Referral, SpecialtyError>.Success)result).Value;
        var names = await SpecialtyNamesAsync(cancellationToken);
        return Ok(ReferralResourceFromEntityAssembler.ToResourceFromEntity(
            referral, names.GetValueOrDefault(referral.SpecialtyId, referral.SpecialtyId.ToString())));
    }

    private async Task<Dictionary<int, string>> SpecialtyNamesAsync(CancellationToken cancellationToken)
    {
        var specialties = await specialtyRepository.ListAsync(cancellationToken);
        return specialties.ToDictionary(s => s.Id, s => s.Name);
    }

    private async Task<Specialty?> FindByNameAsync(string name, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        var specialties = await specialtyRepository.ListAsync(cancellationToken);
        return specialties.FirstOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase)
            || string.Equals(s.Key, name, StringComparison.OrdinalIgnoreCase));
    }

    private ActionResult<ReferralResource> ToErrorResponse(SpecialtyError error) => error.Type switch
    {
        ESpecialtyError.NotFound => NotFound(new { message = error.Message }),
        ESpecialtyError.Conflict => Conflict(new { message = error.Message }),
        _ => BadRequest(new { message = error.Message })
    };
}
