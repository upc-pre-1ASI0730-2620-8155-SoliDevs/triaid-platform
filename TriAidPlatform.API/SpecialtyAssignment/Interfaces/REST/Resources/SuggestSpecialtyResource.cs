namespace TriAidPlatform.API.SpecialtyAssignment.Interfaces.REST.Resources;

/// <summary>Request body of the specialty suggestion query (US30).</summary>
/// <param name="Symptom">Free-text main symptom of the patient.</param>
/// <param name="TriageLevel">Confirmed triage level (1-5).</param>
/// <param name="PatientAge">Patient age in years.</param>
/// <param name="PatientSex">Patient sex code (F/M).</param>
public record SuggestSpecialtyResource(string Symptom, int? TriageLevel, int? PatientAge, string? PatientSex);
