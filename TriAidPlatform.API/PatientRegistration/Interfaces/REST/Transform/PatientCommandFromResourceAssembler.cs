using TriAidPlatform.API.PatientRegistration.Domain.Model.Commands;
using TriAidPlatform.API.PatientRegistration.Domain.Model.ValueObjects;
using TriAidPlatform.API.PatientRegistration.Interfaces.REST.Resources;

namespace TriAidPlatform.API.PatientRegistration.Interfaces.REST.Transform;

/// <summary>Maps REST payloads into Patient Registration commands.</summary>
public static class PatientCommandFromResourceAssembler
{
    /// <summary>Converts a registration payload into a command.</summary>
    /// <exception cref="ArgumentException">Thrown when the sex value is not Male or Female.</exception>
    public static CreatePatientCommand ToCommandFromResource(CreatePatientResource resource) =>
        new(resource.Dni?.Trim() ?? string.Empty, resource.FirstName, resource.LastName, resource.BirthDate,
            ParseSex(resource.Sex), resource.Phone, resource.Address);

    /// <summary>Converts a temporary profile payload into a command.</summary>
    /// <exception cref="ArgumentException">Thrown when the sex value is not Male or Female.</exception>
    public static CreateTemporaryProfileCommand ToCommandFromResource(CreateTemporaryProfileResource resource) =>
        new(resource.FirstName, resource.LastName, resource.BirthDate, ParseSex(resource.Sex));

    /// <summary>Converts a regularization payload into a command.</summary>
    public static RegularizeIdentityCommand ToCommandFromResource(RegularizeIdentityResource resource) =>
        new(resource.Dni?.Trim() ?? string.Empty);

    /// <summary>Converts a contact update payload into a command.</summary>
    public static UpdateContactCommand ToCommandFromResource(UpdateContactResource resource) =>
        new(resource.Phone, resource.Address);

    private static Sex ParseSex(string? value)
    {
        if (Enum.TryParse<Sex>(value, true, out var sex) && Enum.IsDefined(sex) && !int.TryParse(value, out _))
            return sex;
        throw new ArgumentException("The patient sex must be Male or Female.");
    }
}
