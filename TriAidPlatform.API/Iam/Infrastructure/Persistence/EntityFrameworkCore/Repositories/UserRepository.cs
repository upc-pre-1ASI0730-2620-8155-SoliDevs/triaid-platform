using Microsoft.EntityFrameworkCore;
using TriAidPlatform.API.Iam.Domain.Model.Aggregates;
using TriAidPlatform.API.Iam.Domain.Model.ValueObjects;
using TriAidPlatform.API.Iam.Domain.Repositories;
using TriAidPlatform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

namespace TriAidPlatform.API.Iam.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/**
 * <summary>
 *     The user repository
 * </summary>
 * <remarks>
 *     This repository is used to manage users
 * </remarks>
 */
public class UserRepository(AppDbContext context) : IUserRepository
{
    /**
     * <summary>
     *     Find a user by username
     * </summary>
     */
    public Task<User?> FindByUsernameAsync(string username, CancellationToken cancellationToken) =>
        context.Set<User>().FirstOrDefaultAsync(
            u => u.Username.ToLower() == username.ToLower(), cancellationToken);

    /**
     * <summary>
     *     Find a user by username without change tracking
     * </summary>
     */
    public Task<User?> FindByUsernameAsNoTrackingAsync(string username, CancellationToken cancellationToken) =>
        context.Set<User>().AsNoTracking().FirstOrDefaultAsync(
            u => u.Username.ToLower() == username.ToLower(), cancellationToken);

    /**
     * <summary>
     *     Find a user by id without change tracking
     * </summary>
     */
    public Task<User?> FindByIdAsNoTrackingAsync(UserId id, CancellationToken cancellationToken) =>
        context.Set<User>().AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    /**
     * <summary>
     *     Check if a user exists by username
     * </summary>
     */
    public Task<bool> ExistsByUsernameAsync(string username, CancellationToken cancellationToken) =>
        context.Set<User>().AnyAsync(
            u => u.Username.ToLower() == username.ToLower(), cancellationToken);

    /**
     * <summary>
     *     List the staff accounts of a tenant institution
     * </summary>
     */
    public async Task<IEnumerable<User>> ListByInstitutionAsync(string institution, CancellationToken cancellationToken)
    {
        var users = await context.Set<User>()
            .Where(u => u.Institution.ToLower() == institution.ToLower())
            .ToListAsync(cancellationToken);
        return users;
    }

    /**
     * <summary>
     *     Whether the institution already has an administrator
     * </summary>
     */
    public Task<bool> ExistsAdminByInstitutionAsync(string institution, CancellationToken cancellationToken) =>
        context.Set<User>().AnyAsync(
            u => u.Institution.ToLower() == institution.ToLower() && u.IsAdmin, cancellationToken);

    /**
     * <summary>
     *     Count the accounts of a tenant institution (seat usage)
     * </summary>
     */
    public Task<int> CountByInstitutionAsync(string institution, CancellationToken cancellationToken) =>
        context.Set<User>().CountAsync(
            u => u.Institution.ToLower() == institution.ToLower(), cancellationToken);

    /// <inheritdoc />
    public async Task AddAsync(User entity, CancellationToken cancellationToken = default) =>
        await context.Set<User>().AddAsync(entity, cancellationToken);

    /// <inheritdoc />
    public Task<User?> FindByIdAsync(int id, CancellationToken cancellationToken = default) =>
        context.Set<User>().FindAsync([new UserId(id)], cancellationToken).AsTask();

    /// <inheritdoc />
    public void Update(User entity) => context.Set<User>().Update(entity);

    /// <inheritdoc />
    public void Remove(User entity) => context.Set<User>().Remove(entity);

    /// <inheritdoc />
    public async Task<IEnumerable<User>> ListAsync(CancellationToken cancellationToken = default) =>
        await context.Set<User>().ToListAsync(cancellationToken);
}
