namespace TriAidPlatform.API.PatientRegistration.Domain.Model.Commands;

/// <summary>
/// Command to open a new triage episode when a registered patient arrives at the emergency service.
/// </summary>
/// <param name="PatientId">Identifier of the registered patient.</param>
public record OpenTriageEpisodeCommand(int PatientId);
