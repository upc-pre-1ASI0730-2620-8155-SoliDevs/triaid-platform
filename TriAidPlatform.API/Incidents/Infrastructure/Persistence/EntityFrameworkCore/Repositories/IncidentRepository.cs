using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.Incidents.Domain.Model.Aggregates;
using TriAidPlatform.API.Incidents.Domain.Model.ValueObjects;
using TriAidPlatform.API.Incidents.Domain.Repositories;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

namespace TriAidPlatform.API.Incidents.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
/// Infrastructure implementation of the <see cref="IIncidentRepository"/> interface.
/// Provides data access operations for the <see cref="Incident"/> aggregate using Entity Framework Core.
/// Acts as a bridge between the domain rules of the Alerting bounded context and the relational database.
/// </summary>
public class IncidentRepository(AppDbContext context) : BaseRepository<Incident>(context), IIncidentRepository
{
    public async Task<IEnumerable<Incident>> FindActiveIncidentsAsync(ClinicId clinicId, CancellationToken cancellationToken = default)
    {
        var incidents = await Context.Set<Incident>().ToListAsync(cancellationToken);
        return incidents.Where(i => i.ClinicId.Value == clinicId.Value && i.Status.StatusValue != EIncidentStatus.Resolved);
    }

    public async Task<Incident?> FindByReadingIdAsync(ClinicId clinicId, int readingId, CancellationToken cancellationToken = default)
    {
        var incidents = await Context.Set<Incident>().ToListAsync(cancellationToken);
        return incidents.FirstOrDefault(i => i.ClinicId.Value == clinicId.Value && i.ReadingId == readingId);
    }

    public async Task<IEnumerable<Incident>> ListAsync()
    {
        return await Context.Set<Incident>().ToListAsync();
    }

    public async Task<Incident?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Incident>().FindAsync([new IncidentId(id)], cancellationToken);
    }
}