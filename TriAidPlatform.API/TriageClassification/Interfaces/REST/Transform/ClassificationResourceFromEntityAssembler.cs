using TriAidPlatform.API.TriageClassification.Domain.Model.Aggregates;
using TriAidPlatform.API.TriageClassification.Interfaces.REST.Resources;

namespace TriAidPlatform.API.TriageClassification.Interfaces.REST.Transform;

/// <summary>Transforms the Classification aggregate into its REST resource.</summary>
public static class ClassificationResourceFromEntityAssembler
{
    /// <summary>Maps the aggregate state to the REST representation.</summary>
    /// <param name="classification">Aggregate to transform.</param>
    /// <returns>The REST resource.</returns>
    public static ClassificationResource ToResourceFromEntity(Classification classification) => new(
        classification.Id,
        classification.EpisodeId,
        classification.SuggestedLevel,
        classification.FinalLevel,
        classification.State,
        classification.SuggestedAt,
        classification.ConfirmedAt,
        classification.Justification,
        classification.MissingMetrics);
}
