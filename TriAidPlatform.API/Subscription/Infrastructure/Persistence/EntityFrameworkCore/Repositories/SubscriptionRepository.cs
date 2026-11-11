using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using TriAidPlatform.API.Subscription.Domain.Model.Aggregates;
using TriAidPlatform.API.Subscription.Domain.Repositories;
using SubscriptionEntity = TriAidPlatform.API.Subscription.Domain.Model.Aggregates.Subscription;

namespace TriAidPlatform.API.Subscription.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>EF Core implementation of the SubscriptionEntity repository.</summary>
public class SubscriptionRepository(AppDbContext context) : ISubscriptionRepository
{
    public Task<SubscriptionEntity?> FindByIdAsync(string id, CancellationToken cancellationToken = default) =>
        context.Set<SubscriptionEntity>().FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<IEnumerable<SubscriptionEntity>> ListByInstitutionAsync(string institution,
        CancellationToken cancellationToken = default)
    {
        var list = await context.Set<SubscriptionEntity>()
            .Where(s => s.Institution.ToLower() == institution.ToLower())
            .ToListAsync(cancellationToken);
        return list;
    }

    public async Task AddAsync(SubscriptionEntity subscription, CancellationToken cancellationToken = default) =>
        await context.Set<SubscriptionEntity>().AddAsync(subscription, cancellationToken);

    public async Task CompleteAsync(CancellationToken cancellationToken = default) =>
        await context.SaveChangesAsync(cancellationToken);
}
