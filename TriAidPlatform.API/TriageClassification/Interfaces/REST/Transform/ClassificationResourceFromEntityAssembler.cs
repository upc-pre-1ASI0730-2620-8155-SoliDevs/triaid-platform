using TriAidPlatform.API.TriageClassification.Domain.Model.Aggregates;
using TriAidPlatform.API.TriageClassification.Domain.Model.ValueObjects;
using TriAidPlatform.API.TriageClassification.Interfaces.REST.Resources;

namespace TriAidPlatform.API.TriageClassification.Interfaces.REST.Transform;

/// <summary>
///     Transforms the Classification aggregate into its web client
///     resource, translating the internal level and state enums into
///     the shared kernel keys used by the client (ADR-010).
/// </summary>
public static class ClassificationResourceFromEntityAssembler
{
    /// <summary>Maps an internal triage level to the client level key.</summary>
    /// <param name="level">Internal triage level.</param>
    /// <returns>The client level key.</returns>
    public static string ToLevelKey(ETriageLevel level) => level switch
    {
        ETriageLevel.Resuscitation => "I_Resuscitation",
        ETriageLevel.Emergency => "II_Emergency",
        ETriageLevel.Urgent => "III_Urgent",
        ETriageLevel.LessUrgent => "IV_LessUrgent",
        _ => "V_NonUrgent"
    };

    /// <summary>Maps a client level key to the internal triage level.</summary>
    /// <param name="levelKey">Client level key.</param>
    /// <returns>The internal triage level.</returns>
    public static ETriageLevel FromLevelKey(string? levelKey) => levelKey switch
    {
        "I_Resuscitation" => ETriageLevel.Resuscitation,
        "II_Emergency" => ETriageLevel.Emergency,
        "III_Urgent" => ETriageLevel.Urgent,
        "IV_LessUrgent" => ETriageLevel.LessUrgent,
        _ => ETriageLevel.NoUrgency
    };

    /// <summary>Maps an internal classification state to the client state key.</summary>
    /// <param name="state">Internal classification state.</param>
    /// <returns>The client state key.</returns>
    public static string ToStateKey(EClassificationState state) => state switch
    {
        EClassificationState.Suggested => "PendingConfirmation",
        EClassificationState.Confirmed => "Confirmed",
        _ => "Overridden"
    };

    /// <summary>Maps a client state key to the internal classification state.</summary>
    /// <param name="stateKey">Client state key.</param>
    /// <returns>The internal classification state.</returns>
    public static EClassificationState FromStateKey(string? stateKey) => stateKey switch
    {
        "Assigned" => EClassificationState.Suggested,
        "Confirmed" => EClassificationState.Confirmed,
        _ => EClassificationState.Suggested
    };

    /// <summary>Maps the aggregate state to the REST representation.</summary>
    /// <param name="classification">Aggregate to transform.</param>
    /// <returns>The REST resource.</returns>
    public static ClassificationResource ToResourceFromEntity(Classification classification) => new(
        classification.Id,
        classification.EpisodeId,
        ToLevelKey(classification.SuggestedLevel),
        ToLevelKey(classification.FinalLevel),
        ToStateKey(classification.State),
        classification.SuggestedAt,
        classification.ConfirmedAt,
        classification.Justification,
        classification.MissingMetrics,
        classification.OverriddenBy,
        classification.Institution);
}
