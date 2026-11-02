namespace TriAidPlatform.API.TriageClassification.Domain.Model.ValueObjects;

/// <summary>
/// Lifecycle state of a triage classification. The suggestion is computed,
/// optionally confirmed by the nurse, and a manual change requires a
/// clinical justification.
/// </summary>
public enum EClassificationState
{
    /// <summary>The system suggested a level and is waiting for approval.</summary>
    Suggested = 1,

    /// <summary>The nurse confirmed the suggested level (US20).</summary>
    Confirmed = 2,

    /// <summary>The nurse changed the level with justification (US21/US22).</summary>
    Modified = 3
}
