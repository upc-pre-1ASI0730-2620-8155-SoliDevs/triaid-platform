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
    /// <summary>
    /// Retrieves a patient by its national identity document number.
    /// Used by the admission search to autocomplete the patient data.
    /// </summary>
    /// <param name="dni">The DNI to search.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>An asynchronous task returning the <see cref="Patient"/> if found; otherwise, null.</returns>
    public async Task<Patient?> GetByDniAsync(string dni, string institution, CancellationToken cancellationToken = default)
    {
        // The DNI is unique per institution, not across the whole system.
        return await Context.Set<Patient>()
            .FirstOrDefaultAsync(p => p.Dni == dni && p.Institution == institution, cancellationToken);
    }

    /// <summary>
    /// Retrieves a temporary patient by its emergency code.
    /// </summary>
    /// <param name="code">The temporary code to search.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>An asynchronous task returning the <see cref="Patient"/> if found; otherwise, null.</returns>
    public async Task<Patient?> GetByTemporaryCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Patient>()
            .FirstOrDefaultAsync(p => p.TemporaryCode == code, cancellationToken);
    }

    public async Task<IReadOnlyList<Patient>> ListByIdentifierAsync(string identifier, CancellationToken cancellationToken = default)
    {
        // The same person may hold a record at more than one institution; the access
        // code decides which record the patient is signing in to.
        var value = identifier.Trim();
        return await Context.Set<Patient>()
            .Where(p => p.Dni == value || p.Email == value.ToLower())
            .ToListAsync(cancellationToken);
    }
}
