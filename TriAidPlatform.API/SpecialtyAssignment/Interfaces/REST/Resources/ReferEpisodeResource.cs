namespace TriAidPlatform.API.SpecialtyAssignment.Interfaces.REST.Resources;

/// <summary>Request body to refer a classified episode (US31).</summary>
/// <param name="SpecialtyId">Identifier of the destination specialty.</param>
/// <param name="Symptom">Main symptom registered for the episode.</param>
public record ReferEpisodeResource(int SpecialtyId, string Symptom);
