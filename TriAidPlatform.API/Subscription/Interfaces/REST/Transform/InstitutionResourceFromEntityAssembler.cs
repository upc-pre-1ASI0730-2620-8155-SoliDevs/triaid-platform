using TriAidPlatform.API.Subscription.Domain.Model;
using TriAidPlatform.API.Subscription.Interfaces.REST.Resources;

namespace TriAidPlatform.API.Subscription.Interfaces.REST.Transform;

/// <summary>Maps between Institution entities and REST resources.</summary>
public static class InstitutionResourceFromEntityAssembler
{
    /// <summary>Converts an entity into its REST representation.</summary>
    public static InstitutionResource ToResource(Institution entity) =>
        new(entity.Id, entity.Name, entity.Ruc, entity.Address, entity.Phone, entity.Email, entity.Active);
}
