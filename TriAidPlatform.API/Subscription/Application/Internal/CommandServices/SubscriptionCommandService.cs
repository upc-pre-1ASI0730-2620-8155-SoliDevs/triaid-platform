using TriAidPlatform.API.Subscription.Domain.Model.Aggregates;
using TriAidPlatform.API.Subscription.Domain.Repositories;
using TriAidPlatform.API.Subscription.Domain.Services;
using TriAidPlatform.API.Subscription.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using SubscriptionEntity = TriAidPlatform.API.Subscription.Domain.Model.Aggregates.Subscription;

namespace TriAidPlatform.API.Subscription.Application.Internal.CommandServices;

/// <summary>Implements the subscription lifecycle use cases.</summary>
public class SubscriptionCommandService(
    ISubscriptionRepository subscriptionRepository,
    PlanRepository planRepository,
    IPaymentGateway paymentGateway) : ISubscriptionCommandService
{
    private const int TrialDays = 14;

    /// <inheritdoc />
    public async Task<TrialResult> StartTrialAsync(string institution, string planId, string cardNumber,
        CancellationToken cancellationToken = default)
    {
        var subscription = new SubscriptionEntity
        {
            Institution = institution.Trim(),
            PlanId = planId,
            BillingCycle = "monthly",
            Status = "trial",
            StartedAt = DateTime.UtcNow,
            TrialEndsAt = DateTime.UtcNow.AddDays(TrialDays),
            PaymentMethod = MaskCard(cardNumber)
        };
        await subscriptionRepository.AddAsync(subscription, cancellationToken);
        return new TrialResult(subscription.Id, subscription.Institution,
            subscription.PlanId, subscription.Status, subscription.TrialEndsAt);
    }

    /// <inheritdoc />
    public async Task<SubscriptionEntity> CreateAsync(SubscriptionEntity subscription, CancellationToken cancellationToken = default)
    {
        await subscriptionRepository.AddAsync(subscription, cancellationToken);
        return subscription;
    }

    /// <inheritdoc />
    public async Task<ChargeResult> RenewAsync(string subscriptionId, string method,
        CancellationToken cancellationToken = default)
    {
        var subscription = await subscriptionRepository.FindByIdAsync(subscriptionId, cancellationToken);
        if (subscription is null)
            return new ChargeResult(false, subscription!);

        var plan = await planRepository.FindByIdAsync(subscription.PlanId, cancellationToken);
        var price = PriceFor(subscription, plan);
        var result = paymentGateway.Charge(price, string.IsNullOrWhiteSpace(method) ? subscription.PaymentMethod : method);
        subscription.Payments.Add(new Payment
        {
            SubscriptionId = subscription.Id,
            Amount = price,
            Status = result.Status,
            Method = result.Method
        });

        if (result.Approved)
        {
            subscription.Status = "active";
            subscription.RenewalDate = NextRenewal(subscription.RenewalDate, subscription.BillingCycle);
        }
        else
        {
            subscription.Status = "pending";
        }

        await subscriptionRepository.CompleteAsync(cancellationToken);
        return new ChargeResult(result.Approved, subscription);
    }

    /// <inheritdoc />
    public async Task<ChargeResult> AutoChargeAsync(string subscriptionId, CancellationToken cancellationToken = default)
    {
        var subscription = await subscriptionRepository.FindByIdAsync(subscriptionId, cancellationToken);
        if (subscription is null || subscription.Status != "trial" || subscription.TrialEndsAt is null)
            return new ChargeResult(false, subscription!);
        if (DateTime.UtcNow <= subscription.TrialEndsAt)
            return new ChargeResult(false, subscription);

        var plan = await planRepository.FindByIdAsync(subscription.PlanId, cancellationToken);
        var price = PriceFor(subscription, plan);
        var result = paymentGateway.Charge(price, subscription.PaymentMethod);
        subscription.Payments.Add(new Payment
        {
            SubscriptionId = subscription.Id,
            Amount = price,
            Status = result.Status,
            Method = result.Method
        });

        if (result.Approved)
        {
            subscription.Status = "active";
            var renewal = subscription.TrialEndsAt.Value.Date;
            subscription.RenewalDate = NextRenewal(renewal.ToString("yyyy-MM-dd"), subscription.BillingCycle);
        }
        else
        {
            subscription.Status = "pending";
        }

        await subscriptionRepository.CompleteAsync(cancellationToken);
        return new ChargeResult(result.Approved, subscription);
    }

    private static decimal PriceFor(SubscriptionEntity subscription, Plan? plan) =>
        (subscription.BillingCycle == "annual" ? plan?.AnnualPrice : plan?.MonthlyPrice) ?? 0;

    private static string NextRenewal(string current, string billingCycle)
    {
        var date = string.IsNullOrEmpty(current) ? DateTime.UtcNow : DateTime.Parse(current);
        date = billingCycle == "annual" ? date.AddYears(1) : date.AddMonths(1);
        return date.ToString("yyyy-MM-dd");
    }

    private static string MaskCard(string raw) =>
        string.IsNullOrWhiteSpace(raw) ? "Visa •••• 4242" : $"Visa •••• {raw.Replace(" ", "")[^4..]}";
}
