using Microsoft.AspNetCore.Mvc;
using TriAidPlatform.API.Incidents.Application.Internal.CommandServices;
using TriAidPlatform.API.Incidents.Application.Internal.QueryServices;
using TriAidPlatform.API.Incidents.Domain.Model.Queries;
using TriAidPlatform.API.Incidents.Interfaces.Rest.Resources;
using TriAidPlatform.API.Incidents.Interfaces.Rest.Transform;

namespace TriAidPlatform.API.Incidents.Interfaces.Rest;

/// <summary>
///     REST facade of the Alerting bounded context. Exposes the full
///     record CRUD used by the web client: institution-scoped listings,
///     creation from the triage flow and severity-driven state
///     transitions (ADR-010 backend swap contract).
/// </summary>
/// <param name="queryService">Read use cases of the bounded context.</param>
/// <param name="commandService">Write use cases of the bounded context.</param>
[ApiController]
[Route("[controller]")]
[Produces("application/json")]
[Tags("Alerts")]
public class IncidentController(IncidentQueryService queryService, IncidentCommandService commandService) : ControllerBase
{
    /// <summary>Lists every alert across institutions; the web client filters client-side.</summary>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>All alerts.</returns>
    [HttpGet]
    [EndpointSummary("List all alerts")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var alerts = await queryService.Handle(new ListAllIncidentsQuery());
        return Ok(alerts.Select(IncidentResourceFromEntityAssembler.ToResourceFromEntity));
    }

    /// <summary>Retrieves a single alert by identifier.</summary>
    /// <param name="id">Identifier of the alert.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>The alert, or 404 when it does not exist.</returns>
    [HttpGet("{id:int}")]
    [EndpointSummary("Get an alert by id")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var alert = await queryService.Handle(new GetIncidentByIdQuery(id));
        return alert is null
            ? NotFound(new { error = "alert-not-found" })
            : Ok(IncidentResourceFromEntityAssembler.ToResourceFromEntity(alert));
    }

    /// <summary>
    ///     Creates an alert from the web client triage flow: one record per
    ///     out-of-range vital sign plus one per high-priority classification
    /// </summary>
    /// <param name="resource">The alert record sent by the client.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>The created alert with its identifier.</returns>
    /// <response code="200">Alert created.</response>
    /// <response code="400">Invalid payload.</response>
    [HttpPost]
    [EndpointSummary("Create an alert")]
    public async Task<IActionResult> Create([FromBody] CreateIncidentResource resource,
        CancellationToken cancellationToken)
    {
        var command = IncidentCommandFromResourceAssemblers.ToCommandFromResource(resource);
        try
        {
            var alert = await commandService.Handle(command, cancellationToken);
            return Ok(IncidentResourceFromEntityAssembler.ToResourceFromEntity(alert!));
        }
        catch (Exception exception)
        {
            return BadRequest(new { error = exception.Message });
        }
    }

    /// <summary>
    ///     Applies a full-record update from the web client: a severity of
    ///     resolved acknowledges the alert and escalated routes it with a
    ///     destination (ADR-010)
    /// </summary>
    /// <param name="id">Identifier of the alert.</param>
    /// <param name="resource">The alert record sent by the client.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>The updated alert, or 404 when it does not exist.</returns>
    [HttpPut("{id:int}")]
    [EndpointSummary("Update an alert state")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAlertResource resource,
        CancellationToken cancellationToken)
    {
        var command = IncidentCommandFromResourceAssemblers.ToCommandFromResource(id, resource);
        var alert = await commandService.Handle(command, cancellationToken);
        return alert is null
            ? NotFound(new { error = "alert-not-found" })
            : Ok(IncidentResourceFromEntityAssembler.ToResourceFromEntity(alert));
    }
}
