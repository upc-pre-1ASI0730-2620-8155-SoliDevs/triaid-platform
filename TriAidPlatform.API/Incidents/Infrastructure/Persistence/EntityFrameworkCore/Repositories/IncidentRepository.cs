using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.Incidents.Domain.Model.Aggregates;
using TriAidPlatform.API.Incidents.Domain.Model.ValueObjects;
using TriAidPlatform.API.Incidents.Domain.Repositories;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace TriAidPlatform.API.Incidents.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
/// Provides Entity Framework Core persistence operations for <see cref="Incident"/> aggregates.
/// </summary>
/// <remarks>
/// Implements the Incidents repository contract and inherits the basic CRUD operations
/// exposed by <see cref="BaseRepository{Incident}"/>.
/// </remarks>
public class IncidentRepository(AppDbContext context) : BaseRepository<Incident>(context), IIncidentRepository
{
    /// <summary>
    /// Finds incidents that belong to a clinic and whose status is not resolved.
    /// </summary>
    /// <param name="clinicId">The clinic whose incidents should be returned.</param>
    /// <param name="cancellationToken">The token used to cancel the database operation.</param>
    /// <returns>The matching active incidents for the specified clinic.</returns>
    public async Task<IEnumerable<Incident>> FindActiveIncidentsAsync(ClinicId clinicId, CancellationToken cancellationToken = default)
    {
        var incidents = await Context.Set<Incident>().ToListAsync(cancellationToken);
        return incidents.Where(i => i.ClinicId.Value == clinicId.Value && i.Status.StatusValue != EIncidentStatus.Resolved);
    }

    /// <summary>
    /// Finds the incident associated with a reading within a specific clinic.
    /// </summary>
    /// <param name="clinicId">The clinic that owns the incident.</param>
    /// <param name="readingId">The identifier of the vital-sign reading associated with the incident.</param>
    /// <param name="cancellationToken">The token used to cancel the database operation.</param>
    /// <returns>The matching incident, or <see langword="null"/> when no incident is found.</returns>
    public async Task<Incident?> FindByReadingIdAsync(ClinicId clinicId, int readingId, CancellationToken cancellationToken = default)
    {
        var incidents = await Context.Set<Incident>().ToListAsync(cancellationToken);
        return incidents.FirstOrDefault(i => i.ClinicId.Value == clinicId.Value && i.ReadingId == readingId);
    }

    /// <summary>
    /// Retrieves every incident from the database.
    /// </summary>
    /// <returns>All persisted incident aggregates.</returns>
    public async Task<IEnumerable<Incident>> ListAsync()
    {
        return await Context.Set<Incident>().ToListAsync();
    }

    /// <summary>
    /// Retrieves an incident using its strongly typed identifier.
    /// </summary>
    /// <param name="id">The underlying integer identifier of the incident.</param>
    /// <param name="cancellationToken">The token used to cancel the database operation.</param>
    /// <returns>The matching incident, or <see langword="null"/> when no incident is found.</returns>
    public async Task<Incident?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Incident>().FindAsync([new IncidentId(id)], cancellationToken);
    }
}