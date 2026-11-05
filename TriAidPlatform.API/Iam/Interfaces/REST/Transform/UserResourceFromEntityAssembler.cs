using TriAidPlatform.API.Iam.Domain.Model.Aggregates;
using TriAidPlatform.API.Iam.Interfaces.REST.Resources;

namespace TriAidPlatform.API.Iam.Interfaces.REST.Transform;

/// <summary>Maps between User aggregates and REST resources.</summary>
public static class UserResourceFromEntityAssembler
{
    /// <summary>Converts a user entity into its safe REST representation.</summary>
    public static UserResource ToResource(User entity) =>
        new(entity.Id, entity.Username, entity.Names, entity.Surnames,
            entity.Email, entity.Role, entity.Unit, entity.Shift, entity.Institution);
}
