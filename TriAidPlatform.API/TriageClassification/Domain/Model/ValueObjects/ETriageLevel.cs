namespace TriAidPlatform.API.TriageClassification.Domain.Model.ValueObjects;

/// <summary>
/// Clinical triage priority levels defined by Health Technical Standard
/// No. 158-MINSA/DIGEPRES. Persisted as the level_id column that references
/// the triage_levels catalog.
/// </summary>
public enum ETriageLevel
{
    /// <summary>Resuscitation: immediate life-saving attention.</summary>
    Resuscitation = 1,

    /// <summary>Emergency: attention within minutes.</summary>
    Emergency = 2,

    /// <summary>Urgent: attention within hours.</summary>
    Urgent = 3,

    /// <summary>Less urgent: attention can be deferred.</summary>
    LessUrgent = 4,

    /// <summary>No urgency: guidance-level care.</summary>
    NoUrgency = 5
}
