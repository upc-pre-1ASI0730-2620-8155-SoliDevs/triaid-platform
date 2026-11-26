namespace TriAidPlatform.API.Iam.Domain.Model.Aggregates;

/// <summary>
/// Identity carried by a patient portal session. Distinct from <see cref="User"/>:
/// a patient is not staff, has no username and never sees the staff panel.
/// </summary>
/// <param name="PatientId">Patient aggregate identifier.</param>
/// <param name="FullName">Display name for the portal header.</param>
/// <param name="Identifier">DNI or e-mail the patient signed in with.</param>
public record PatientSession(int PatientId, string FullName, string Identifier)
{
    /// <summary>Role claim placed in patient tokens; never accepted by staff endpoints.</summary>
    public const string Role = "Patient";
}
