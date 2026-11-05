namespace TriAidPlatform.API.Alerting.Domain.Model.ValueObjects;

public record ClinicId
{
    public int Value { get; }
    
    public ClinicId(int value)
    {
        if (value <= 0) 
            throw new ArgumentException("Clinic ID must be a valid identifier to ensure data isolation.");
        
        Value = value;
    }
}