using TriAidPlatform.API.Iam.Domain.Model.Aggregates;
using TriAidPlatform.API.Iam.Domain.Model.Commands;
using TriAidPlatform.API.Shared.Application.Model;

namespace TriAidPlatform.API.Iam.Application.Internal.CommandServices;

/// <summary>
/// Authenticates patients for the patient portal. Separate from
/// <see cref="IUserCommandService"/> so staff and patient sessions never mix.
/// </summary>
public interface IPatientSignInService
{
    /// <summary>
    /// Validates the identifier and access code and returns the patient session
    /// together with its JWT token.
    /// </summary>
    /// <param name="command">Sign-in credentials.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<Result<(PatientSession Session, string Token)>> Handle(
        PatientSignInCommand command, CancellationToken cancellationToken = default);
}
