using TriAidPlatform.API.Subscription.Domain.Model.Aggregates;
using SubscriptionEntity = TriAidPlatform.API.Subscription.Domain.Model.Aggregates.Subscription;

namespace TriAidPlatform.API.Subscription.Domain.Repositories;

/// <summary>Repository contract for the SubscriptionEntity aggregate.</summary>
public interface ISubscriptionRepository
{
    /// <summary>Finds a subscription by identifier.</summary>
    Task<SubscriptionEntity?> FindByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>Lists the subscriptions of an institution.</summary>
    Task<IEnumerable<SubscriptionEntity>> ListByInstitutionAsync(string institution,
        CancellationToken cancellationToken = default);

    /// <summary>Lists every subscription across institutions.</summary>
    Task<IEnumerable<SubscriptionEntity>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>Inserts a subscription.</summary>
    Task AddAsync(SubscriptionEntity subscription, CancellationToken cancellationToken = default);

    /// <summary>Persists changes.</summary>
    Task CompleteAsync(CancellationToken cancellationToken = default);
}
