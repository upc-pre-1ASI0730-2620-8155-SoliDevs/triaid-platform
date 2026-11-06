using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Aggregates;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Repositories;

namespace TriAidPlatform.API.SpecialtyAssignment.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>Infrastructure implementation of the Specialty catalog repository.</summary>
/// <param name="context">Database context provided via dependency injection.</param>
public class SpecialtyRepository(AppDbContext context) : BaseRepository<Specialty>(context), ISpecialtyRepository
{
    /// <inheritdoc />
    public async Task<Specialty?> FindByKeyAsync(string key, CancellationToken cancellationToken = default) =>
        await Context.Set<Specialty>()
            .FirstOrDefaultAsync(s => s.Key == key, cancellationToken);
}
