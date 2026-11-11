using TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Aggregates;
using TriAidPlatform.API.Shared.Domain.Repositories;

namespace TriAidPlatform.API.SpecialtyAssignment.Domain.Repositories;

/// <summary>Repository interface of the Specialty catalog entity.</summary>
public interface ISpecialtyRepository : IBaseRepository<Specialty>
{
    /// <summary>Finds a specialty by its stable key.</summary>
    /// <param name="key">Stable key of the specialty.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    /// <returns>The specialty, or null when the key is unknown.</returns>
    Task<Specialty?> FindByKeyAsync(string key, CancellationToken cancellationToken = default);
}
