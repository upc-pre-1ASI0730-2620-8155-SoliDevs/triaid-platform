using Microsoft.AspNetCore.Mvc;
using TriAidPlatform.API.TriageClassification.Application.Internal.CommandServices;
using TriAidPlatform.API.TriageClassification.Application.Internal.QueryServices;
using TriAidPlatform.API.TriageClassification.Domain.Model.Commands;
using TriAidPlatform.API.TriageClassification.Domain.Model.ValueObjects;
using TriAidPlatform.API.TriageClassification.Domain.Services;
using TriAidPlatform.API.TriageClassification.Interfaces.REST.Resources;
using TriAidPlatform.API.TriageClassification.Interfaces.REST.Transform;

namespace TriAidPlatform.API.TriageClassification.Interfaces.REST;

/// <summary>
/// REST facade of the Triage Classification bounded context
/// (US19-US23, US38). Exposes the NT-158 suggestion, the nurse
/// confirmation, the justified manual override and the query endpoints
/// under /triage-classifications.
/// </summary>
/// <param name="classificationService">Write use cases of the bounded context.</param>
/// <param name="queryService">Read use cases of the bounded context.</param>
[ApiController]
[Route("triage-classifications")]
[Produces("application/json")]
public class ClassificationsController(
    IClassificationService classificationService,
    ClassificationQueryService queryService) : ControllerBase
{
    /// <summary>Records the NT-158 suggestion for an episode (US19).</summary>
    /// <param name="episodeId">Identifier of the episode.</param>
    /// <param name="resource">Vital signs captured for the episode.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>The created classification with the suggested level.</returns>
    [HttpPost("episode/{episodeId}")]
    public async Task<ActionResult<ClassificationResource>> Suggest(string episodeId,
        [FromBody] SuggestClassificationResource resource, CancellationToken cancellationToken)
    {
        var readings = SuggestCommandFromResourceAssembler.ToReadingsMap(resource);
        var summary = NT158Engine.Summarize(readings);
        var result = await classificationService.SuggestAsync(episodeId, summary, cancellationToken);
        return result.Fold<ActionResult<ClassificationResource>>(
            classification => Ok(ClassificationResourceFromEntityAssembler.ToResourceFromEntity(classification)),
            ToErrorResponse);
    }

    /// <summary>Approves the suggested level (US20).</summary>
    /// <param name="classificationId">Identifier of the classification.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>The confirmed classification.</returns>
    [HttpPost("{classificationId:int}/confirm")]
    public async Task<ActionResult<ClassificationResource>> Confirm(int classificationId,
        CancellationToken cancellationToken)
    {
        var result = await classificationService.ConfirmAsync(classificationId, cancellationToken);
        return result.Fold<ActionResult<ClassificationResource>>(
            classification => Ok(ClassificationResourceFromEntityAssembler.ToResourceFromEntity(classification)),
            ToErrorResponse);
    }

    /// <summary>Overrides the level with a written justification (US21/US22).</summary>
    /// <param name="classificationId">Identifier of the classification.</param>
    /// <param name="resource">New level and justification.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>The modified classification.</returns>
    [HttpPost("{classificationId:int}/modify")]
    public async Task<ActionResult<ClassificationResource>> Modify(int classificationId,
        [FromBody] ModifyClassificationResource resource, CancellationToken cancellationToken)
    {
        if (!Enum.IsDefined(typeof(ETriageLevel), resource.NewLevel))
            return BadRequest(new { message = "The triage level must be between 1 and 5." });
        var command = new ModifyClassificationCommand(
            classificationId, (ETriageLevel)resource.NewLevel, resource.Justification);
        var result = await classificationService.ModifyAsync(
            command.ClassificationId, command.NewLevel, command.Justification, cancellationToken);
        return result.Fold<ActionResult<ClassificationResource>>(
            classification => Ok(ClassificationResourceFromEntityAssembler.ToResourceFromEntity(classification)),
            ToErrorResponse);
    }

    /// <summary>Retrieves the classification of an episode.</summary>
    /// <param name="episodeId">Identifier of the episode.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>The classification, or 404 when the episode is not classified.</returns>
    [HttpGet("episode/{episodeId}")]
    public async Task<ActionResult<ClassificationResource>> GetByEpisode(string episodeId,
        CancellationToken cancellationToken)
    {
        var classification = await queryService.GetByEpisodeAsync(episodeId, cancellationToken);
        return classification is null
            ? NotFound(new { message = $"Episode with id {episodeId} is not classified yet." })
            : Ok(ClassificationResourceFromEntityAssembler.ToResourceFromEntity(classification));
    }

    /**
     * <summary>
     *     Gets the classification of an episode through the query-string
     *     style used by the web client (ADR-010 backend swap contract)
     * </summary>
     * <param name="episodeId">The episode identifier taken from the query</param>
     * <param name="cancellationToken">The cancellation token</param>
     * <returns>The classification of the episode</returns>
     * <response code="200">Classification found</response>
     * <response code="404">Episode is not classified yet</response>
     */
    [HttpGet]
    public async Task<ActionResult<ClassificationResource>> GetByEpisodeQuery(
        [FromQuery(Name = "episodeId")] string episodeId, CancellationToken cancellationToken)
    {
        return await GetByEpisode(episodeId, cancellationToken);
    }

    private ActionResult<ClassificationResource> ToErrorResponse(ClassificationError error) => error.Type switch
    {
        EClassificationError.NotFound => NotFound(new { message = error.Message }),
        EClassificationError.Conflict => Conflict(new { message = error.Message }),
        _ => BadRequest(new { message = error.Message })
    };
}
