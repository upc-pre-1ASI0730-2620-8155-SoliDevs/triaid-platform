using TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Aggregates;
using TriAidPlatform.API.SpecialtyAssignment.Interfaces.REST.Resources;

namespace TriAidPlatform.API.SpecialtyAssignment.Interfaces.REST.Transform;

/// <summary>Transforms the Referral aggregate into its REST resource.</summary>
public static class ReferralResourceFromEntityAssembler
{
    /// <summary>Maps the aggregate state to the REST representation.</summary>
    /// <param name="referral">Aggregate to transform.</param>
    /// <returns>The REST resource.</returns>
    public static ReferralResource ToResourceFromEntity(Referral referral) =>
        ReferralResource.FromEntity(referral);
}
