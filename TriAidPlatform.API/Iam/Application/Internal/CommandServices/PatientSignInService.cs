using TriAidPlatform.API.Iam.Application.Internal.OutboundServices;
using TriAidPlatform.API.Iam.Domain.Model;
using TriAidPlatform.API.Iam.Domain.Model.Aggregates;
using TriAidPlatform.API.Iam.Domain.Model.Commands;
using TriAidPlatform.API.Iam.Domain.Model.Errors;
using TriAidPlatform.API.Shared.Application.Model;
using TriAidPlatform.API.Shared.Domain.Model.Errors;

namespace TriAidPlatform.API.Iam.Application.Internal.CommandServices;

/// <summary>
/// Authenticates a patient against the Patient Registration directory and issues
/// a short-lived portal token scoped to that single patient.
/// </summary>
/// <param name="patientDirectory">Read-only patient lookup.</param>
/// <param name="hashingService">BCrypt verifier for the portal access code.</param>
/// <param name="tokenService">JWT issuer.</param>
public class PatientSignInService(
    IPatientDirectory patientDirectory,
    IHashingService hashingService,
    ITokenService tokenService) : IPatientSignInService
{
    private static readonly Error PortalNotEnabled =
        Error.Forbidden("Iam.PortalNotEnabled", "The patient portal is not enabled for this patient. Ask at admission.");

    /// <inheritdoc />
    public async Task<Result<(PatientSession Session, string Token)>> Handle(
        PatientSignInCommand command, CancellationToken cancellationToken = default)
    {
        // One person may hold records at several institutions; the access code picks the record.
        var candidates = await patientDirectory.FindAllAsync(command.Identifier, cancellationToken);
        if (candidates.Count == 0)
            return Result<(PatientSession, string)>.Failure(IamErrors.InvalidCredentials, IamError.InvalidCredentials);
        var credentials = candidates.FirstOrDefault(c => c.PasswordHash is not null && hashingService.VerifyPassword(command.Password, c.PasswordHash));
        if (credentials is null)
        {
            if (candidates.All(c => c.PasswordHash is null))
                return Result<(PatientSession, string)>.Failure(PortalNotEnabled, IamError.NotEnoughPermissions);
            return Result<(PatientSession, string)>.Failure(IamErrors.InvalidCredentials, IamError.InvalidCredentials);
        }

        var session = new PatientSession(credentials.PatientId, credentials.FullName, credentials.Identifier);
        var token = tokenService.GeneratePatientToken(session);
        return Result<(PatientSession, string)>.Success((session, token));
    }
}