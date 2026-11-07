namespace TriAidPlatform.API.Alerting.Domain.Model.ValueObjects;
/// <summary>
/// Defines the specific states an alert can transition through during a triage episode.
/// </summary>
public enum EAlertStatus
{
    Active,
    Acknowledged,
    Escalated,
    Resolved
}