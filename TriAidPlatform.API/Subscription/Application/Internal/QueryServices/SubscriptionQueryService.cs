using TriAidPlatform.API.Subscription.Domain.Model.Aggregates;
using TriAidPlatform.API.Subscription.Domain.Repositories;
using TriAidPlatform.API.Subscription.Domain.Services;
using TriAidPlatform.API.Subscription.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using SubscriptionEntity = TriAidPlatform.API.Subscription.Domain.Model.Aggregates.Subscription;

namespace TriAidPlatform.API.Subscription.Application.Internal.QueryServices;

/// <summary>Implements the subscription and catalog queries.</summary>
public class SubscriptionQueryService(
    ISubscriptionRepository subscriptionRepository,
    PlanRepository planRepository) : ISubscriptionQueryService
{
    /// <inheritdoc />
    public async Task<IEnumerable<SubscriptionEntity>> ListByInstitutionAsync(string institution,
        CancellationToken cancellationToken = default) =>
        await subscriptionRepository.ListByInstitutionAsync(institution, cancellationToken);

    /// <inheritdoc />
    public Task<SubscriptionEntity?> FindByIdAsync(string id, CancellationToken cancellationToken = default) =>
        subscriptionRepository.FindByIdAsync(id, cancellationToken);

    /// <inheritdoc />
    public async Task<Plan?> FindCurrentPlanAsync(string institution, CancellationToken cancellationToken = default)
    {
        var subscription = (await subscriptionRepository.ListByInstitutionAsync(institution, cancellationToken))
            .FirstOrDefault();
        return subscription is null ? null : await planRepository.FindByIdAsync(subscription.PlanId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Plan>> ListPlansAsync(CancellationToken cancellationToken = default) =>
        await planRepository.ListAsync(cancellationToken);
}
