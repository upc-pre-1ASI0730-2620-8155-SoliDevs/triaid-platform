using TriAidPlatform.API.TriageClassification.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.TriageClassification.Domain.Model.Commands;

/// <summary>Command that overrides the level with justification (US21/US22).</summary>
/// <param name="ClassificationId">Identifier of the classification.</param>
/// <param name="NewLevel">Clinically justified level.</param>
/// <param name="Justification">Mandatory written reason.</param>
public record ModifyClassificationCommand(int ClassificationId, ETriageLevel NewLevel, string Justification);
