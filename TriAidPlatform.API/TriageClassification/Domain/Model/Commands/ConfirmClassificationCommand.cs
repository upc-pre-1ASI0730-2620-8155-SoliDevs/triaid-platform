namespace TriAidPlatform.API.TriageClassification.Domain.Model.Commands;

/// <summary>Command that approves the suggested level (US20).</summary>
/// <param name="ClassificationId">Identifier of the classification.</param>
public record ConfirmClassificationCommand(int ClassificationId);
