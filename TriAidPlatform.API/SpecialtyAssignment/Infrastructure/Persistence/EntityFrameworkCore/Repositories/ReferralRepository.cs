using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Aggregates;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Repositories;

namespace TriAidPlatform.API.SpecialtyAssignment.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>Infrastructure implementation of the Referral aggregate repository.</summary>
/// <param name="context">Database context provided via dependency injection.</param>
public class ReferralRepository(AppDbContext context) : BaseRepository<Referral>(context), IReferralRepository
{
    /// <inheritdoc />
    public async Task<Referral?> FindByEpisodeIdAsync(string episodeId, CancellationToken cancellationToken = default) =>
        await Context.Set<Referral>()
            .FirstOrDefaultAsync(r => r.EpisodeId == episodeId, cancellationToken);

    /// <inheritdoc />
    public async Task<int> CountWaitingBySpecialtyAsync(int specialtyId, CancellationToken cancellationToken = default) =>
        await Context.Set<Referral>()
            .CountAsync(r => r.SpecialtyId == specialtyId && r.State == "waiting", cancellationToken);
}
