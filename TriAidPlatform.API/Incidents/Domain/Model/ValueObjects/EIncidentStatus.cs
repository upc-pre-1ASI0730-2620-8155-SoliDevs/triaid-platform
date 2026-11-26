namespace TriAidPlatform.API.Incidents.Domain.Model.ValueObjects;
/// <summary>
/// Defines the specific states an incident can transition through during a triage episode.
/// </summary>
public enum EIncidentStatus
{
    Active,
    Acknowledged,
    Escalated,
    Resolved
}