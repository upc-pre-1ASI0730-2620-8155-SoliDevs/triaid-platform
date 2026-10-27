namespace TriAidPlatform.API.Alerting.Domain.Model.Aggregates;

public enum AlertStatus
{
    Active,
    Acknowledged,
    Escalated,
    Resolved    
}