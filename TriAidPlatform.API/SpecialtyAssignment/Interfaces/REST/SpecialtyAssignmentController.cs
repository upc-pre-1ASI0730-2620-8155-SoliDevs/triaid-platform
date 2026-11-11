using Microsoft.AspNetCore.Mvc;
using TriAidPlatform.API.Shared.Application.Patterns;
using TriAidPlatform.API.SpecialtyAssignment.Application.Internal.CommandServices;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Commands;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Queries;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Services;
using TriAidPlatform.API.SpecialtyAssignment.Interfaces.REST.Resources;
using TriAidPlatform.API.SpecialtyAssignment.Interfaces.REST.Transform;

namespace TriAidPlatform.API.SpecialtyAssignment.Interfaces.REST;

/// <summary>
/// REST facade of the Specialty Assignment bounded context
/// (US30-US33, US37). Exposes the specialty catalog, the rule-based
/// suggestion, the referral with queue position and the justified
/// reassignment.
/// </summary>
/// <param name="assignmentService">Use cases of the bounded context.</param>
[ApiController]
[Route("specialty-assignment")]
[Produces("application/json")]
public class SpecialtyAssignmentController(ISpecialtyAssignmentService assignmentService) : ControllerBase
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
            0, resource.Symptom, resource.TriageLevel, resource.PatientAge, resource.PatientSex);
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
    [HttpPost("episode/{episodeId:int}/refer")]
    public async Task<ActionResult<ReferralResource>> Refer(int episodeId,
        [FromBody] ReferEpisodeResource resource, CancellationToken cancellationToken)
    {
        var command = new ReferEpisodeCommand(episodeId, resource.SpecialtyId);
        var result = await assignmentService.ReferAsync(command, resource.Symptom, cancellationToken);
        return result.Fold<ActionResult<ReferralResource>>(
            referral => Ok(ReferralResourceFromEntityAssembler.ToResourceFromEntity(referral)),
            ToErrorResponse);
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
        return result.Fold<ActionResult<ReferralResource>>(
            referral => Ok(ReferralResourceFromEntityAssembler.ToResourceFromEntity(referral)),
            ToErrorResponse);
    }

    /// <summary>Retrieves the referral of an episode with its queue position (US33, US37).</summary>
    /// <param name="episodeId">Identifier of the episode.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>The referral, or 404 when the episode is not referred.</returns>
    [HttpGet("episode/{episodeId:int}/referral")]
    public async Task<ActionResult<ReferralResource>> GetReferral(int episodeId,
        CancellationToken cancellationToken)
    {
        var result = await assignmentService.GetByEpisodeAsync(episodeId, cancellationToken);
        return result.Fold<ActionResult<ReferralResource>>(
            referral => Ok(ReferralResourceFromEntityAssembler.ToResourceFromEntity(referral)),
            ToErrorResponse);
    }

    private ActionResult<ReferralResource> ToErrorResponse(SpecialtyError error) => error.Type switch
    {
        ESpecialtyError.NotFound => NotFound(new { message = error.Message }),
        ESpecialtyError.Conflict => Conflict(new { message = error.Message }),
        _ => BadRequest(new { message = error.Message })
    };
}
