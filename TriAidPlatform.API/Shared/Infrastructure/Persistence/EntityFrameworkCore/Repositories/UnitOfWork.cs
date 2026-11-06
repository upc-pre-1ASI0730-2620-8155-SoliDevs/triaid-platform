using TriAidPlatform.API.Shared.Domain.Repositories;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

namespace TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class UnitOfWork (AppDbContext context) : IUnitOfWork
{
    public async Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}