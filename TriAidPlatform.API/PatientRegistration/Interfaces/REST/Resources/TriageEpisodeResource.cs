namespace TriAidPlatform.API.PatientRegistration.Interfaces.REST.Resources;

/// <summary>Triage episode data returned by the API.</summary>
/// <param name="Id">Episode identifier (episode_id used by the other bounded contexts).</param>
/// <param name="PatientId">Identifier of the patient.</param>
/// <param name="ArrivalTime">UTC arrival date and time.</param>
/// <param name="State">Current state: Waiting, InTriage, Attended or Referred.</param>
public record TriageEpisodeResource(int Id, int PatientId, DateTime ArrivalTime, string State, string? Vitals);
