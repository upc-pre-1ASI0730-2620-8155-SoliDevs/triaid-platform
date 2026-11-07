using TriAidPlatform.API.Alerting.Domain.Model.Aggregates;
using TriAidPlatform.API.Alerting.Interfaces.Rest.Resources;

namespace TriAidPlatform.API.Alerting.Interfaces.Rest.Transform;

/// <summary>
/// Assembler responsible for transforming the <see cref="Alert"/> aggregate into an <see cref="AlertResource"/>.
/// Follows the transform pattern to separate domain models from API contracts.
/// </summary>
public static class AlertResourceFromEntityAssembler
{
    /// <summary>
    /// Projects an <see cref="Alert"/> domain entity into its corresponding REST resource representation.
    /// Safely flattens the metric value object and formats dates to standard ISO 8601 strings.
    /// </summary>
    /// <param name="entity">The source domain entity.</param>
    /// <returns>A new <see cref="AlertResource"/> instance containing the mapped data.</returns>
    public static AlertResource ToResourceFromEntity(Alert entity)
    {
        return new AlertResource(
            entity.Id,
            entity.ClinicId.Value,
            entity.Status.StatusValue.ToString(),
            entity.Level.ToString(),
            entity.ReadingId,
            entity.RaisedAt.ToString("O"),
            entity.AcknowledgedAt?.ToString("O"),
            entity.EscalatedTo,
            entity.Metric?.SignName ?? string.Empty,
            entity.Metric?.MeasuredValue ?? string.Empty,
            entity.Metric?.SafeRange ?? string.Empty
        );
    }
}