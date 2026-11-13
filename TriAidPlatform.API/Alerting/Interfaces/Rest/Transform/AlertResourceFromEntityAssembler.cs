using TriAidPlatform.API.Alerting.Domain.Model.Aggregates;
using TriAidPlatform.API.Alerting.Domain.Model.ValueObjects;
using TriAidPlatform.API.Alerting.Interfaces.Rest.Resources;

namespace TriAidPlatform.API.Alerting.Interfaces.Rest.Transform;

/// <summary>
/// Assembler responsible for transforming the <see cref="Alert"/> aggregate into an <see cref="AlertResource"/>.
/// Keeps the domain model separate from the REST contract by mapping alert, patient, triage, and metric data
/// to the resource's flattened representation.
/// </summary>
public static class AlertResourceFromEntityAssembler
{
    /// <summary>
    /// Projects an <see cref="Alert"/> aggregate into its REST resource representation.
    /// Derives the resource severity from the alert status and level, formats the measurement time as
    /// <c>HH:mm</c>, and formats the acknowledgment, escalation, and creation timestamps using round-trip
    /// (<c>o</c>) format. Missing metric values are represented as empty strings.
    /// </summary>
    /// <param name="entity">The alert aggregate to map.</param>
    /// <returns>A new <see cref="AlertResource"/> populated with the alert's mapped data.</returns>
    public static AlertResource ToResourceFromEntity(Alert entity)
    {
        string severityString = entity.Status.StatusValue switch
        {
            EAlertStatus.Active when entity.Level == EAlertLevel.Critical => "critical",
            EAlertStatus.Active when entity.Level == EAlertLevel.Warning => "warning",
            EAlertStatus.Acknowledged => "resolved",
            EAlertStatus.Resolved => "resolved",
            EAlertStatus.Escalated => "escalated",
            _ => "warning"
        };
        return new AlertResource(
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