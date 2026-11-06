using TriAidPlatform.API.PatientRegistration.Domain.Model.Aggregates;
using TriAidPlatform.API.PatientRegistration.Interfaces.REST.Resources;

namespace TriAidPlatform.API.PatientRegistration.Interfaces.REST.Transform;

/// <summary>Maps Patient entities into REST resources.</summary>
public static class PatientResourceFromEntityAssembler
{
    /// <summary>Converts an entity into its REST representation.</summary>
    public static PatientResource ToResourceFromEntity(Patient entity) =>
        new(entity.Id, entity.Dni, entity.FirstName, entity.LastName, entity.BirthDate,
            entity.Sex.ToString(), entity.Phone, entity.Address, entity.IsTemporary, entity.TemporaryCode);
}
