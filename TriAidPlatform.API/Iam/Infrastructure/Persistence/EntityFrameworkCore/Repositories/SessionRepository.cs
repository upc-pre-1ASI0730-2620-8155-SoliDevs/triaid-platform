using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.Iam.Domain.Model.Aggregates;
using TriAidPlatform.API.Iam.Domain.Repositories;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

namespace TriAidPlatform.API.Iam.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>EF Core implementation of the Session repository.</summary>
public class SessionRepository(AppDbContext context) : ISessionRepository
{
    public Task<Session?> FindByIdAsync(string token, CancellationToken cancellationToken = default) =>
        context.Set<Session>().FirstOrDefaultAsync(s => s.Id == token, cancellationToken);

    public async Task AddAsync(Session session, CancellationToken cancellationToken = default) =>
        await context.Set<Session>().AddAsync(session, cancellationToken);

    public void Remove(Session session) => context.Set<Session>().Remove(session);

    public async Task CompleteAsync(CancellationToken cancellationToken = default) =>
        await context.SaveChangesAsync(cancellationToken);
}
