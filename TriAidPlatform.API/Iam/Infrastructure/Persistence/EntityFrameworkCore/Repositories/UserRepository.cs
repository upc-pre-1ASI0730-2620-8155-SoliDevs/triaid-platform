using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.Iam.Domain.Model.Aggregates;
using TriAidPlatform.API.Iam.Domain.Repositories;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

namespace TriAidPlatform.API.Iam.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>EF Core implementation of the User repository.</summary>
public class UserRepository(AppDbContext context) : IUserRepository
{
    public Task<User?> FindByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        context.Set<User>().FirstOrDefaultAsync(
            u => u.Username.ToLower() == username.ToLower(), cancellationToken);

    public Task<User?> FindByIdAsync(int id, CancellationToken cancellationToken = default) =>
        context.Set<User>().FindAsync([id], cancellationToken).AsTask();

    public async Task<IEnumerable<User>> ListByInstitutionAsync(string institution, CancellationToken cancellationToken = default)
    {
        var users = await context.Set<User>()
            .Where(u => u.Institution.ToLower() == institution.ToLower())
            .ToListAsync(cancellationToken);
        return users;
    }

    public Task<bool> ExistsAdminAsync(string institution, CancellationToken cancellationToken = default) =>
        context.Set<User>().AnyAsync(
            u => u.Institution.ToLower() == institution.ToLower() && u.IsAdmin, cancellationToken);

    public Task<bool> ExistsUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        context.Set<User>().AnyAsync(
            u => u.Username.ToLower() == username.ToLower(), cancellationToken);

    public Task<int> CountByInstitutionAsync(string institution, CancellationToken cancellationToken = default) =>
        context.Set<User>().CountAsync(
            u => u.Institution.ToLower() == institution.ToLower(), cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default) =>
        await context.Set<User>().AddAsync(user, cancellationToken);

    public async Task CompleteAsync(CancellationToken cancellationToken = default) =>
        await context.SaveChangesAsync(cancellationToken);
}
