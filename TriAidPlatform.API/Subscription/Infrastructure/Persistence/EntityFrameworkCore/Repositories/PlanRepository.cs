using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using TriAidPlatform.API.Subscription.Domain.Model.Aggregates;
using TriAidPlatform.API.Subscription.Domain.Repositories;

namespace TriAidPlatform.API.Subscription.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>EF Core implementation of the plan catalog repository.</summary>
public class PlanRepository(AppDbContext context)
{
    /// <summary>Lists every plan of the catalog.</summary>
    public Task<List<Plan>> ListAsync(CancellationToken cancellationToken = default) =>
        context.Set<Plan>().OrderBy(p => p.MonthlyPrice).ToListAsync(cancellationToken);

    /// <summary>Finds a plan by its natural identifier.</summary>
    public Task<Plan?> FindByIdAsync(string id, CancellationToken cancellationToken = default) =>
        context.Set<Plan>().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    /// <summary>Inserts a plan (seed).</summary>
    public async Task AddAsync(Plan plan, CancellationToken cancellationToken = default) =>
        await context.Set<Plan>().AddAsync(plan, cancellationToken);

    /// <summary>Persists changes.</summary>
    public async Task CompleteAsync(CancellationToken cancellationToken = default) =>
        await context.SaveChangesAsync(cancellationToken);
}
