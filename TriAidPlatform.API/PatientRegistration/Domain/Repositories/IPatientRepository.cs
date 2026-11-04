using TriAidPlatform.API.PatientRegistration.Domain.Model.Aggregates;
using TriAidPlatform.API.Shared.Domain.Repositories;

namespace TriAidPlatform.API.PatientRegistration.Domain.Repositories;

/// <summary>
/// Repository interface for the Patient aggregate root.
/// Inherits basic CRUD operations from IBaseRepository and defines patient-specific lookups.
/// </summary>
public interface IPatientRepository : IBaseRepository<Patient>
{
    /// <summary>
    /// Retrieves a patient by its national identity document number.
    /// </summary>
    /// <param name="dni">The DNI to search.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The <see cref="Patient"/> if found; otherwise, null.</returns>
    Task<Patient?> GetByDniAsync(string dni, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a temporary patient by its emergency code.
    /// </summary>
    /// <param name="code">The temporary code to search.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The <see cref="Patient"/> if found; otherwise, null.</returns>
    Task<Patient?> GetByTemporaryCodeAsync(string code, CancellationToken cancellationToken = default);
}
