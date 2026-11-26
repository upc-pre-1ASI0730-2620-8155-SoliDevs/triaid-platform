using TriAidPlatform.API.Iam.Application.Internal.OutboundServices;
using TriAidPlatform.API.PatientRegistration.Domain.Repositories;

namespace TriAidPlatform.API.PatientRegistration.Application.Internal.OutboundServices;

/// <summary>
/// Exposes the minimum patient identity IAM needs to authenticate portal
/// sessions, without leaking the Patient aggregate or its repository.
/// </summary>
/// <param name="patientRepository">Patient repository.</param>
public class PatientDirectory(IPatientRepository patientRepository) : IPatientDirectory
{
    /// <inheritdoc />
    public async Task<PatientCredentials?> FindAsync(string identifier, CancellationToken cancellationToken = default)
    {
        var patient = await patientRepository.GetByIdentifierAsync(identifier, cancellationToken);
        if (patient is null) return null;

        return new PatientCredentials(
            patient.Id,
            $"{patient.FirstName} {patient.LastName}".Trim(),
            patient.Email is not null && identifier.Trim().Equals(patient.Email, StringComparison.OrdinalIgnoreCase)
                ? patient.Email
                : patient.Dni ?? identifier.Trim(),
            patient.PortalPasswordHash);
    }
}
