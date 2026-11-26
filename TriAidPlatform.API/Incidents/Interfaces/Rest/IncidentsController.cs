using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using TriAidPlatform.API.Incidents.Application.CommandServices;
using TriAidPlatform.API.Incidents.Application.QueryServices;
using TriAidPlatform.API.Incidents.Domain.Model.Commands;
using TriAidPlatform.API.Incidents.Domain.Model.Queries;
using TriAidPlatform.API.Incidents.Interfaces.Rest.Resources;
using TriAidPlatform.API.Incidents.Interfaces.Rest.Transform;

namespace TriAidPlatform.API.Incidents.Interfaces.Rest;

/// <summary>
/// API Controller for managing the clinical lifecycle of patient incidents.
/// </summary>
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Incidents")]
public class IncidentsController(
    IIncidentCommandService incidentCommandService,
    IIncidentQueryService incidentQueryService) : ControllerBase
{
    [HttpGet]
    [EndpointSummary("Get All Incidents")]
    [EndpointDescription("Retrieves all registered incidents in the system.")]
    [ProducesResponseType(typeof(IEnumerable<IncidentResource>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAllIncidents(CancellationToken cancellationToken)
    {
        var incidents = await incidentQueryService.Handle(new ListAllIncidentsQuery(), cancellationToken);
        var resources = incidents.Select(IncidentResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet("{id:int}")]
    [EndpointSummary("Get Incident by Id")]
    [EndpointDescription("Retrieves a specific incident by its unique identifier.")]
    [ProducesResponseType(typeof(IncidentResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetIncidentById([FromRoute] int id, CancellationToken cancellationToken)
    {
        var incident = await incidentQueryService.Handle(new GetIncidentByIdQuery(id), cancellationToken);

        return IncidentsActionResultAssembler.ToActionResultFromGetIncidentByIdResult(
            this,
            incident,
            found => Ok(IncidentResourceFromEntityAssembler.ToResourceFromEntity(found))
        );
    }

    [HttpPost]
    [EndpointSummary("Create Incident")]
    [EndpointDescription("Creates a new clinical incident from triage vital signs or priority classification.")]
    [ProducesResponseType(typeof(IncidentResource), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateIncident([FromBody] CreateIncidentResource resource, CancellationToken cancellationToken)
    {
        var command = IncidentCommandFromResourceAssemblers.ToCommandFromResource(resource);
        var result = await incidentCommandService.Handle(command, cancellationToken);

        return IncidentsActionResultAssembler.ToActionResultFromCommandResult(
            this,
            result,
            created => CreatedAtAction(
                nameof(GetIncidentById),
                new { id = created.Id.Value },
                IncidentResourceFromEntityAssembler.ToResourceFromEntity(created))
        );
    }

    [HttpPut("{id:int}")]
    [EndpointSummary("Update Incident")]
    [EndpointDescription("Updates an incident status (acknowledge/resolve or escalate to a critical care area).")]
    [ProducesResponseType(typeof(IncidentResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateIncident(
        [FromRoute] int id,
        [FromBody] UpdateIncidentResource resource,
        CancellationToken cancellationToken)
    {
        var command = new UpdateIncidentCommand(id, resource.Severity, resource.EscalatedTo);
        var result = await incidentCommandService.Handle(command, cancellationToken);

        return IncidentsActionResultAssembler.ToActionResultFromCommandResult(
            this,
            result,
            updated => Ok(IncidentResourceFromEntityAssembler.ToResourceFromEntity(updated))
        );
    }
}