using Microsoft.AspNetCore.Mvc;
using TriAidPlatform.API.Subscription.Application.Internal.QueryServices;

namespace TriAidPlatform.API.Subscription.Interfaces.REST;

/// <summary>Public catalog of commercial plans (no authentication).</summary>
[ApiController]
[Route("plans")]
public class PlansController(SubscriptionQueryService queryService) : ControllerBase
{
    /// <summary>Lists every plan ordered by monthly price.</summary>
    /// <response code="200">Plan catalog.</response>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await queryService.ListPlansAsync(cancellationToken));
}
