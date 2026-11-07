using Microsoft.AspNetCore.Mvc;
using TriAidPlatform.API.Alerting.Application.Internal.CommandServices;
using TriAidPlatform.API.Alerting.Application.Internal.QueryServices;
using TriAidPlatform.API.Alerting.Domain.Model.Commands;
using TriAidPlatform.API.Alerting.Domain.Model.Queries;
using TriAidPlatform.API.Alerting.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.Alerting.Interfaces.Rest;


[ApiController]
[Route("api/v1/triage/{clinicId}/[controller]")]
[Produces("application/json")]
public class AlertsController(AlertQueryService queryService, AlertCommandService commandService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetActiveAlerts([FromRoute] int clinicId, CancellationToken cancellationToken)
    {
        var query = new GetActiveAlertsQuery(new ClinicId(clinicId));
        var result = await queryService.Handle(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{alertId}/acknowledge")]
    public async Task<IActionResult> AcknowledgeAlert([FromRoute] int clinicId, [FromRoute] int alertId, CancellationToken cancellationToken)
    {
        var command = new AcknowledgeAlertCommand(new ClinicId(clinicId), alertId);
        var result = await commandService.Handle(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{alertId}/escalate")]
    public async Task<IActionResult> EscalateAlert([FromRoute] int clinicId, [FromRoute] int alertId, [FromBody] EscalateRequest request, CancellationToken cancellationToken)
    {
        var command = new EscalateAlertCommand(new ClinicId(clinicId), alertId, request.Destination);
        var result = await commandService.Handle(command, cancellationToken);
        return Ok(result);
    }
}

public record EscalateRequest(string Destination);