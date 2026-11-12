using Microsoft.AspNetCore.Mvc;
using TriAidPlatform.API.Shared.Domain.Repositories;
using TriAidPlatform.API.TriageClassification.Application.Internal.CommandServices;
using TriAidPlatform.API.TriageClassification.Application.Internal.QueryServices;
using TriAidPlatform.API.TriageClassification.Domain.Model.Aggregates;
using TriAidPlatform.API.TriageClassification.Domain.Model.Commands;
using TriAidPlatform.API.TriageClassification.Domain.Model.Queries;
using TriAidPlatform.API.TriageClassification.Domain.Model.ValueObjects;
using TriAidPlatform.API.TriageClassification.Domain.Repositories;
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
/// <param name="classificationRepository">Repository of the Classification aggregate.</param>
/// <param name="unitOfWork">Unit of work used to commit the changes.</param>
[ApiController]
[Route("triage-classifications")]
[Produces("application/json")]
public class ClassificationsController(
    IClassificationService classificationService,
    ClassificationQueryService queryService,
    IClassificationRepository classificationRepository,
    IUnitOfWork unitOfWork) : ControllerBase
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
    public async Task<ActionResult<IEnumerable<ClassificationResource>>> GetByEpisodeQuery(
        [FromQuery(Name = "episodeId")] string? episodeId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(episodeId))
        {
            var all = await classificationRepository.ListAsync(cancellationToken);
            return Ok(all.Select(ClassificationResourceFromEntityAssembler.ToResourceFromEntity));
        }
        var single = await GetByEpisode(episodeId, cancellationToken);
        if (single.Result is not null) return new ActionResult<IEnumerable<ClassificationResource>>(single.Result);
        return Ok(new List<ClassificationResource> { single.Value! });
    }

    /**
     * <summary>
     *     Creates a classification record from the web client. The
     *     client reports the computed level and state; the NT-158
     *     engine endpoint above stays for API-only clients (ADR-010)
     * </summary>
     * <param name="resource">The classification record sent by the client.</param>
     * <param name="cancellationToken">Token to observe for cancellation.</param>
     * <returns>The created classification.</returns>
     * <response code="200">Classification created.</response>
     * <response code="409">Episode already classified.</response>
     */
    [HttpPost]
    public async Task<ActionResult<ClassificationResource>> CreateRecord(
        [FromBody] SaveClassificationResource resource, CancellationToken cancellationToken)
    {
        if (await classificationRepository.FindByEpisodeIdAsync(resource.EpisodeId, cancellationToken) is not null)
            return Conflict(new { message = $"Episode with id {resource.EpisodeId} is already classified." });

        var classification = new Classification(new SuggestClassificationCommand(
                resource.EpisodeId,
                ClassificationResourceFromEntityAssembler.FromLevelKey(resource.SuggestedLevel),
                string.IsNullOrEmpty(resource.MissingMetrics) ? [] : [resource.MissingMetrics]))
            .ApplyRecord(
                ClassificationResourceFromEntityAssembler.FromLevelKey(resource.SuggestedLevel),
                ClassificationResourceFromEntityAssembler.FromLevelKey(resource.Level),
                ClassificationResourceFromEntityAssembler.FromStateKey(resource.State),
                resource.ConfirmedAt, resource.Justification, resource.OverriddenBy, resource.Institution);
        await classificationRepository.AddAsync(classification, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return Ok(ClassificationResourceFromEntityAssembler.ToResourceFromEntity(classification));
    }

    /**
     * <summary>
     *     Applies a full classification record update from the web
     *     client: level in force, state, justification and audit
     *     fields (ADR-010)
     * </summary>
     * <param name="id">Identifier of the classification.</param>
     * <param name="resource">The classification record sent by the client.</param>
     * <param name="cancellationToken">Token to observe for cancellation.</param>
     * <returns>The updated classification.</returns>
     * <response code="200">Classification updated.</response>
     * <response code="404">Classification not found.</response>
     */
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ClassificationResource>> UpdateRecord(int id,
        [FromBody] SaveClassificationResource resource, CancellationToken cancellationToken)
    {
        var classification = await classificationRepository.FindByIdAsync(id, cancellationToken);
        if (classification is null)
            return NotFound(new { message = $"Classification with id {id} was not found." });

        classification.ApplyRecord(
            ClassificationResourceFromEntityAssembler.FromLevelKey(resource.SuggestedLevel),
            ClassificationResourceFromEntityAssembler.FromLevelKey(resource.Level),
            ClassificationResourceFromEntityAssembler.FromStateKey(resource.State),
            resource.ConfirmedAt, resource.Justification, resource.OverriddenBy, resource.Institution);
        await unitOfWork.CompleteAsync(cancellationToken);
        var updated = ClassificationResourceFromEntityAssembler.ToResourceFromEntity(classification);
        return Ok(updated with { State = resource.State });
    }

    private ActionResult<ClassificationResource> ToErrorResponse(ClassificationError error) => error.Type switch
    {
        EClassificationError.NotFound => NotFound(new { message = error.Message }),
        EClassificationError.Conflict => Conflict(new { message = error.Message }),
        _ => BadRequest(new { message = error.Message })
    };
}
