namespace TriAidPlatform.API.TriageClassification.Domain.Model.ValueObjects;

/// <summary>Error types produced by the classification use cases.</summary>
public enum EClassificationError
{
    /// <summary>The referenced resource does not exist.</summary>
    NotFound,

    /// <summary>The operation conflicts with the classification lifecycle.</summary>
    Conflict,

    /// <summary>A mandatory input is missing or invalid.</summary>
    Validation
}
