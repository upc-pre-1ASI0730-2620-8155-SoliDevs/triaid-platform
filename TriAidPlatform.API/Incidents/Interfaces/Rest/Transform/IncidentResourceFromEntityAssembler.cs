using TriAidPlatform.API.Incidents.Domain.Model.Aggregates;
using TriAidPlatform.API.Incidents.Domain.Model.ValueObjects;
using TriAidPlatform.API.Incidents.Interfaces.Rest.Resources;

namespace TriAidPlatform.API.Incidents.Interfaces.Rest.Transform;

/// <summary>
/// Assembler responsible for transforming the <see cref="Incident"/> aggregate into an <see cref="IncidentResource"/>.
/// Keeps the domain model separate from the REST contract by mapping incident, patient, triage, and metric data
/// to the resource's flattened representation.
/// </summary>
public static class IncidentResourceFromEntityAssembler
{
    /// <summary>
    /// Projects an <see cref="Incident"/> aggregate into its REST resource representation.
    /// Derives the resource severity from the incident status and level, formats the measurement time as
    /// <c>HH:mm</c>, and formats the acknowledgment, escalation, and creation timestamps using round-trip
    /// (<c>o</c>) format. Missing metric values are represented as empty strings.
    /// </summary>
    /// <param name="entity">The incident aggregate to map.</param>
    /// <returns>A new <see cref="IncidentResource"/> populated with the incident's mapped data.</returns>
    public static IncidentResource ToResourceFromEntity(Incident entity)
    {
        string severityString = entity.Status.StatusValue switch
        {
            EIncidentStatus.Active when entity.Level == EIncidentLevel.Critical => "critical",
            EIncidentStatus.Active when entity.Level == EIncidentLevel.Warning => "warning",
            EIncidentStatus.Acknowledged => "resolved",
            EIncidentStatus.Resolved => "resolved",
            EIncidentStatus.Escalated => "escalated",
            _ => "warning"
        };
        return new IncidentResource(
            entity.Id,
            entity.Institution,
            entity.PatientName,
            entity.Dni,
            entity.EpisodeId, 
            entity.Priority,
            entity.Metric?.SignName ?? string.Empty,
            entity.Metric?.MeasuredValue ?? string.Empty,
            entity.Metric?.SafeRange ?? string.Empty,
            entity.RaisedAt.ToString("HH:mm"),
            severityString, 
            entity.AcknowledgedAt?.ToString("o"),
            entity.EscalatedAt?.ToString("o"),
            entity.EscalatedTo,
            entity.RaisedAt.ToString("o") 
        );
    }
}