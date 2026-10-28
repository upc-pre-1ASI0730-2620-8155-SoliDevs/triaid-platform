using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using TriAidPlatform.API.Subscription.Domain.Model;
using TriAidPlatform.API.Subscription.Domain.Repositories;

namespace TriAidPlatform.API.Subscription.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/**
 * EF Core implementation of the Institution repository. Persistence is
 * fully delegated to the shared BaseRepository and UnitOfWork.
 */
public class InstitutionRepository(AppDbContext context)
    : BaseRepository<Institution>(context), IInstitutionRepository
{
    public Task<Institution?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return Context.Set<Institution>()
            .FirstOrDefaultAsync(i => i.Name == name, cancellationToken);
    }

    public Task<bool> ExistsByRucAsync(string ruc, CancellationToken cancellationToken = default)
    {
        return Context.Set<Institution>()
            .AnyAsync(i => i.Ruc == ruc, cancellationToken);
    }

    public async Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        await Context.SaveChangesAsync(cancellationToken);
    }
}
