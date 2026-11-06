namespace TriAidPlatform.API.Alerting.Domain.Model.ValueObjects;

/// <summary>
/// Value object representing the lifecycle state of a medical alert.
/// Encapsulates the state value and provides business rules for state transitions.
/// </summary>
public sealed record AlertStatus
{
    /// <summary>
    /// Gets the current state of the alert.
    /// </summary>
    public EAlertStatus StatusValue { get; init; }
    
    /// <summary>
    /// Private parameterless constructor strictly required by Entity Framework Core for object materialization.
    /// </summary>
    private AlertStatus() { }
    
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AlertStatus"/> value object.
    /// </summary>
    /// <param name="statusValue">The initial lifecycle state to assign.</param>
    public AlertStatus(EAlertStatus statusValue)
    {
        StatusValue = statusValue;
    }
    
    /// <summary>
    /// Determines whether the current alert state represents the end of its lifecycle.
    /// </summary>
    /// <returns>True if the alert is resolved; otherwise, false.</returns>
    public bool isFinalState() => StatusValue == EAlertStatus.Resolved;
    
}


