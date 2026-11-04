namespace TriAidPlatform.API.TriageClassification.Interfaces.REST.Resources;

/// <summary>Request body to override the level with justification (US21/US22).</summary>
/// <param name="NewLevel">Clinically justified level (1-5).</param>
/// <param name="Justification">Mandatory written reason.</param>
public record ModifyClassificationResource(int NewLevel, string Justification);
