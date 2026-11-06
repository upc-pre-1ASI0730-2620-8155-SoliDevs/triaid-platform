namespace TriAidPlatform.API.PatientRegistration.Interfaces.REST.Resources;

/// <summary>Payload to open a triage episode.</summary>
/// <param name="PatientId">Identifier of the registered patient (required).</param>
public record OpenTriageEpisodeResource(int PatientId);
