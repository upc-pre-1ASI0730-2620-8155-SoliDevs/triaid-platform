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
    Task<Patient?> GetByDniAsync(string dni, string institution, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a temporary patient by its emergency code.
    /// </summary>
    /// <param name="code">The temporary code to search.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The <see cref="Patient"/> if found; otherwise, null.</returns>
    Task<Patient?> GetByTemporaryCodeAsync(string code, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a patient by portal identifier: the DNI, or the contact e-mail when it looks like one.
    /// </summary>
    /// <param name="identifier">DNI or e-mail entered at the portal sign-in.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<IReadOnlyList<Patient>> ListByIdentifierAsync(string identifier, CancellationToken cancellationToken = default);
}
