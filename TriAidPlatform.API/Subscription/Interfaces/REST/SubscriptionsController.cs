using Microsoft.AspNetCore.Mvc;
using TriAidPlatform.API.Iam.Domain.Model.Aggregates;
using TriAidPlatform.API.Iam.Domain.Repositories;
using TriAidPlatform.API.Subscription.Application.Internal.CommandServices;
using TriAidPlatform.API.Subscription.Domain.Services;
using TriAidPlatform.API.Subscription.Application.Internal.QueryServices;
using TriAidPlatform.API.Subscription.Domain.Model.Aggregates;
using TriAidPlatform.API.Subscription.Domain.Repositories;
using SubscriptionEntity = TriAidPlatform.API.Subscription.Domain.Model.Aggregates.Subscription;

namespace TriAidPlatform.API.Subscription.Interfaces.REST;

/// <summary>
/// REST facade of the SubscriptionEntity bounded context. Shapes match the
/// web application client: plain arrays for plans and tenant-scoped
/// subscriptions, plus full-record PUT updates from the checkout flow.
/// </summary>
[ApiController]
[Route("subscriptions")]
public class SubscriptionsController(
    ISubscriptionCommandService commandService,
    ISubscriptionQueryService queryService,
    ISubscriptionRepository subscriptionRepository) : ControllerBase
{
    /// <summary>
    ///     Resolves the authenticated user set by the IAM authorization
    ///     middleware.
    /// </summary>
    private TriAidPlatform.API.Iam.Domain.Model.Aggregates.User? CurrentUser() =>
        (TriAidPlatform.API.Iam.Domain.Model.Aggregates.User?)HttpContext.Items["User"];

    /// <summary>
    ///     Lists subscriptions. Authenticated callers get their tenant;
    ///     anonymous callers get the whole listing so the web client
    ///     can filter by institution client-side (ADR-010).
    /// </summary>
    /// <response code="200">Subscriptions (may be empty).</response>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var caller = CurrentUser();
        if (caller is null)
            return Ok(await subscriptionRepository.ListAsync(cancellationToken));
        return Ok(await queryService.ListByInstitutionAsync(caller.Institution, cancellationToken));
    }

    /// <summary>
    ///     Registers a subscription (checkout activation flow). The
    ///     institution comes from the token when present, otherwise
    ///     from the record itself (ADR-010).
    /// </summary>
    /// <response code="200">SubscriptionEntity registered.</response>
    /// <response code="400">Missing institution.</response>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SaveSubscriptionResource body, CancellationToken cancellationToken)
    {
        var caller = CurrentUser();
        var entity = new SubscriptionEntity
        {
            Institution = caller?.Institution ?? body.Institution,
            PlanId = body.PlanId,
            BillingCycle = body.BillingCycle,
            Status = body.Status,
            StartedAt = body.StartedAt,
            TrialEndsAt = body.TrialEndsAt,
            RenewalDate = body.RenewalDate,
            GraceDays = body.GraceDays,
            PaymentMethod = body.PaymentMethod
        };
        if (string.IsNullOrWhiteSpace(entity.Institution))
            return BadRequest(new { error = "missing-institution" });

        entity.Payments.AddRange(body.Payments.Select(p => new Payment
        {
            Amount = p.Amount,
            Status = p.Status,
            Method = p.Method,
            PaidAt = p.PaidAt ?? DateTime.UtcNow,
            SubscriptionId = entity.Id
        }));
        var created = await commandService.CreateAsync(entity, cancellationToken);
        await subscriptionRepository.CompleteAsync(cancellationToken);
        return Ok(created);
    }

    /// <summary>
    ///     Full-record update used by the checkout retry flow. Tenant
    ///     checks apply only when a token is present (ADR-010).
    /// </summary>
    /// <response code="200">SubscriptionEntity updated.</response>
    /// <response code="404">SubscriptionEntity not found in the tenant.</response>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] SubscriptionEntity body, CancellationToken cancellationToken)
    {
        var caller = CurrentUser();

        var target = await subscriptionRepository.FindByIdAsync(id, cancellationToken);
        if (target is null)
            return NotFound();
        if (caller is not null && !string.Equals(target.Institution, caller.Institution, StringComparison.OrdinalIgnoreCase))
            return NotFound();

        target.Status = body.Status;
        target.RenewalDate = body.RenewalDate;
        target.BillingCycle = body.BillingCycle;
        if (body.Payments.Count > 0) target.Payments = body.Payments;

        await subscriptionRepository.CompleteAsync(cancellationToken);
        return Ok(target);
    }

    /// <summary>
    ///     Charges the next cycle of the contract (renewal flow). Tenant
    ///     checks apply only when a token is present (ADR-010).
    /// </summary>
    /// <response code="200">Charge outcome with the updated subscription.</response>
    /// <response code="404">SubscriptionEntity not found in the tenant.</response>
    [HttpPost("{id}/renew")]
    public async Task<IActionResult> Renew(string id, [FromBody] RenewResource body, CancellationToken cancellationToken)
    {
        var caller = CurrentUser();

        var sub = await queryService.FindByIdAsync(id, cancellationToken);
        if (sub is null)
            return NotFound();
        if (caller is not null && !string.Equals(sub.Institution, caller.Institution, StringComparison.OrdinalIgnoreCase))
            return NotFound();

        var result = await commandService.RenewAsync(id, body?.Method ?? string.Empty, cancellationToken);
        return Ok(new { rejected = !result.Approved, subscription = result.SubscriptionEntity });
    }

    /// <summary>
    ///     Runs the automatic trial-end charge when due. Tenant checks
    ///     apply only when a token is present (ADR-010).
    /// </summary>
    /// <response code="200">Charge outcome with the updated subscription.</response>
    /// <response code="404">SubscriptionEntity not found in the tenant.</response>
    [HttpPost("{id}/auto-charge")]
    public async Task<IActionResult> AutoCharge(string id, CancellationToken cancellationToken)
    {
        var caller = CurrentUser();

        var sub = await queryService.FindByIdAsync(id, cancellationToken);
        if (sub is null)
            return NotFound();
        if (caller is not null && !string.Equals(sub.Institution, caller.Institution, StringComparison.OrdinalIgnoreCase))
            return NotFound();

        var result = await commandService.AutoChargeAsync(id, cancellationToken);
        return Ok(new { charged = result.Approved, subscription = result.SubscriptionEntity });
    }
}

/// <summary>Renewal charge payload.</summary>
/// <param name="Method">Masked card to charge; empty keeps the stored one.</param>
public record RenewResource(string? Method);

/**
 * <summary>
 *     Subscription body used by the web client checkout flow, tolerant
 *     of null identifiers on creation (ADR-010).
 * </summary>
 * <param name="Id">Client-side identifier, null on creation.</param>
 * <param name="Institution">Institution the subscription belongs to.</param>
 * <param name="PlanId">Identifier of the contracted plan.</param>
 * <param name="BillingCycle">Monthly or annual billing cycle.</param>
 * <param name="Status">Lifecycle status (trial, active, pending, overdue).</param>
 * <param name="StartedAt">UTC start timestamp.</param>
 * <param name="TrialEndsAt">UTC trial end timestamp.</param>
 * <param name="RenewalDate">Next renewal date.</param>
 * <param name="GraceDays">Grace period days after expiry.</param>
 * <param name="PaymentMethod">Masked payment method on file.</param>
 * <param name="Payments">Payment records of the subscription.</param>
 */
public record SaveSubscriptionResource(
    string? Id,
    string Institution,
    string PlanId,
    string BillingCycle,
    string Status,
    DateTime? StartedAt,
    DateTime? TrialEndsAt,
    string RenewalDate,
    int GraceDays,
    string PaymentMethod,
    List<SavePaymentResource> Payments);

/**
 * <summary>
 *     Payment body tolerant of a null identifier on creation.
 * </summary>
 * <param name="Amount">Charged amount.</param>
 * <param name="Status">Payment status (paid, rejected).</param>
 * <param name="Method">Masked card used for the charge.</param>
 * <param name="PaidAt">UTC timestamp of the charge.</param>
 */
public record SavePaymentResource(decimal Amount, string Status, string Method, DateTime? PaidAt);
