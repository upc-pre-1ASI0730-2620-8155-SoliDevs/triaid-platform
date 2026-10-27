namespace TriAidPlatform.API.Alerting.Domain.Model.Aggregates;
/// <summary>
/// Represents the lifecycle states of a medical alert within the system.
/// </summary>
public enum AlertStatus
{
    Active,
    Acknowledged,
    Escalated,
    Resolved    
}