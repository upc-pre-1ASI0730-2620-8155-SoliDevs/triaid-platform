using TriAidPlatform.API.Subscription.Domain.Model.Aggregates;
using SubscriptionEntity = TriAidPlatform.API.Subscription.Domain.Model.Aggregates.Subscription;

namespace TriAidPlatform.API.Subscription.Domain.Services;

/// <summary>Outcome of a charge applied to a subscription.</summary>
/// <param name="Approved">Whether the charge went through.</param>
/// <param name="SubscriptionEntity">Updated subscription.</param>
public record ChargeResult(bool Approved, SubscriptionEntity SubscriptionEntity);

/// <summary>Created trial subscription.</summary>
/// <param name="Id">SubscriptionEntity identifier.</param>
/// <param name="Institution">Tenant institution.</param>
/// <param name="PlanId">Contracted plan.</param>
/// <param name="Status">Initial status ('trial').</param>
/// <param name="TrialEndsAt">When the trial ends (UTC).</param>
public record TrialResult(string Id, string Institution, string PlanId, string Status, DateTime? TrialEndsAt);

/// <summary>
/// Command service of the SubscriptionEntity bounded context: contract
/// lifecycle (trial, activation, renewal, automatic trial-end charge).
/// </summary>
public interface ISubscriptionCommandService
{
    /// <summary>Starts a 14-day card-required trial for an institution.</summary>
    Task<TrialResult> StartTrialAsync(string institution, string planId, string cardNumber,
        CancellationToken cancellationToken = default);

    /// <summary>Registers a brand-new subscription (activation or pending).</summary>
    Task<SubscriptionEntity> CreateAsync(SubscriptionEntity subscription, CancellationToken cancellationToken = default);

    /// <summary>Charges the renewal of a cycle; failure moves the contract to pending.</summary>
    Task<ChargeResult> RenewAsync(string subscriptionId, string method, CancellationToken cancellationToken = default);

    /// <summary>
    /// Runs the automatic trial-end charge when due: approval activates
    /// the contract, rejection leaves it pending (grace flow).
    /// </summary>
    Task<ChargeResult> AutoChargeAsync(string subscriptionId, CancellationToken cancellationToken = default);
}

/// <summary>Query service of the SubscriptionEntity bounded context.</summary>
public interface ISubscriptionQueryService
{
    /// <summary>Lists the subscriptions of an institution.</summary>
    Task<IEnumerable<SubscriptionEntity>> ListByInstitutionAsync(string institution,
        CancellationToken cancellationToken = default);

    /// <summary>Finds a subscription by identifier.</summary>
    Task<SubscriptionEntity?> FindByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>Finds the contracted plan of an institution (may be null).</summary>
    Task<Plan?> FindCurrentPlanAsync(string institution, CancellationToken cancellationToken = default);

    /// <summary>Lists every plan of the catalog.</summary>
    Task<IEnumerable<Plan>> ListPlansAsync(CancellationToken cancellationToken = default);
}
