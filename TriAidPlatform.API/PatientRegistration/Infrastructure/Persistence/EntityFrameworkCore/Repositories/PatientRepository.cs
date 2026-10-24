using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.PatientRegistration.Domain.Model.Aggregates;
using TriAidPlatform.API.PatientRegistration.Domain.Repositories;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace TriAidPlatform.API.PatientRegistration.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
/// Infrastructure implementation of the <see cref="IPatientRepository"/> interface.
/// Provides data access operations for the <see cref="Patient"/> aggregate using Entity Framework Core.
/// </summary>
/// <param name="context">The database context provided via dependency injection.</param>
public class PatientRepository(AppDbContext context) : BaseRepository<Patient>(context), IPatientRepository
{
    /// <inheritdoc />
    public async Task<Patient?> GetByDniAsync(string dni, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Patient>()
            .FirstOrDefaultAsync(p => p.Dni == dni, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Patient?> GetByTemporaryCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Patient>()
            .FirstOrDefaultAsync(p => p.TemporaryCode == code, cancellationToken);
    }
}
