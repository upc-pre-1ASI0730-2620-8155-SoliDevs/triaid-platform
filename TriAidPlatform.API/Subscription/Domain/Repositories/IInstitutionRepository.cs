using TriAidPlatform.API.Shared.Domain.Repositories;
using TriAidPlatform.API.Subscription.Domain.Model;

namespace TriAidPlatform.API.Subscription.Domain.Repositories;

/**
 * Repository contract for the Institution aggregate. Extends the shared
 * CRUD operations with institution-specific lookups.
 */
public interface IInstitutionRepository : IBaseRepository<Institution>
{
    /// <summary>Finds an institution by its exact registered name.</summary>
    Task<Institution?> FindByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>Reports whether an institution exists with the given RUC.</summary>
    Task<bool> ExistsByRucAsync(string ruc, CancellationToken cancellationToken = default);

    /// <summary>Persists unit of work changes.</summary>
    Task CompleteAsync(CancellationToken cancellationToken = default);
}
