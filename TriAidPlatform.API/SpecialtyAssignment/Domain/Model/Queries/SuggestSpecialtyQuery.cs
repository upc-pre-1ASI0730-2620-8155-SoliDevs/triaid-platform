namespace TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Queries;

/// <summary>Query that computes the suggested specialty for an episode (US30).</summary>
/// <param name="EpisodeId">Identifier of the classified episode.</param>
/// <param name="Symptom">Free-text main symptom of the patient.</param>
/// <param name="TriageLevel">Confirmed triage level (1-5).</param>
/// <param name="PatientAge">Patient age in years.</param>
/// <param name="PatientSex">Patient sex code (F/M).</param>
public record SuggestSpecialtyQuery(string EpisodeId, string Symptom, int? TriageLevel, int? PatientAge, string? PatientSex);
