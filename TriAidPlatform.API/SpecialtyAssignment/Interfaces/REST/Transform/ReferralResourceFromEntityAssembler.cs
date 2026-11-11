using TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Aggregates;
using TriAidPlatform.API.SpecialtyAssignment.Interfaces.REST.Resources;

namespace TriAidPlatform.API.SpecialtyAssignment.Interfaces.REST.Transform;

/// <summary>Transforms the Referral aggregate into its web client resource.</summary>
public static class ReferralResourceFromEntityAssembler
{
    /// <summary>Maps the aggregate state to the REST representation.</summary>
    /// <param name="referral">Aggregate to transform.</param>
    /// <param name="specialtyName">Catalog name of the destination specialty.</param>
    /// <returns>The REST resource.</returns>
    public static ReferralResource ToResourceFromEntity(Referral referral, string specialtyName) =>
        new(referral.Id, referral.EpisodeId, specialtyName, referral.Room,
            referral.QueuePosition, referral.State, referral.ChangeReason, referral.ReferredAt,
            referral.Symptom, referral.Institution);
}
