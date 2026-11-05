namespace TriAidPlatform.API.Alerting.Domain.Model.ValueObjects;

/// <summary>
/// Value Object that represents the unique identifier of a Clinic.
/// Prevents Primitive Obsession and ensures valid tenant identifiers.
/// </summary>
public sealed record ClinicId
{
    public int Value { get; }
    
    public ClinicId(int value)
    {
        if (value <= 0) 
            throw new ArgumentException("Clinic ID must be a valid identifier to ensure data isolation.");
        
        Value = value;
    }
}