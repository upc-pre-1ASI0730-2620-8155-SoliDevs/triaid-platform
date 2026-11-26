using TriAidPlatform.API.Iam.Domain.Model.Commands;
using TriAidPlatform.API.Iam.Interfaces.Rest.Resources;

namespace TriAidPlatform.API.Iam.Interfaces.Rest.Transform;

/// <summary>Maps the patient portal sign-in payload into its command.</summary>
public static class PatientSignInCommandFromResourceAssembler
{
    /// <summary>Converts the sign-in payload into a command.</summary>
    public static PatientSignInCommand ToCommandFromResource(PatientSignInResource resource) =>
        new(resource.Identifier?.Trim() ?? string.Empty, resource.Password ?? string.Empty);
}
