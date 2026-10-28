namespace TriAidPlatform.API.PatientRegistration.Domain.Model.ValueObjects;

/// <summary>
/// Lifecycle state of a triage episode. Persisted as the state_id column that
/// references the episode_states catalog.
/// </summary>
public enum EEpisodeState
{
    /// <summary>The patient arrived and waits to be triaged.</summary>
    Waiting = 1,

    /// <summary>The patient is being evaluated in triage.</summary>
    InTriage = 2,

    /// <summary>The patient was attended in the emergency service.</summary>
    Attended = 3,

    /// <summary>The patient was referred to a specialty.</summary>
    Referred = 4
}
