namespace TriAidPlatform.API.TriageClassification.Domain.Model.ValueObjects;

/// <summary>Structured error of a classification use case failure.</summary>
/// <param name="Type">Error category.</param>
/// <param name="Message">Human readable explanation.</param>
public sealed record ClassificationError(EClassificationError Type, string Message);
