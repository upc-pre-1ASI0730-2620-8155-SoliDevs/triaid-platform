using TriAidPlatform.API.Alerting.Domain.Model.Commands;
using TriAidPlatform.API.Alerting.Domain.Model.Aggregates;
using TriAidPlatform.API.Alerting.Domain.Model.ValueObjects;
using TriAidPlatform.API.Alerting.Interfaces.Rest.Resources;

namespace TriAidPlatform.API.Alerting.Interfaces.Rest.Transform;

/// <summary>
///     Transforms the web client alert records into the bounded context
///     commands (ADR-010 backend swap contract).
/// </summary>
public static class AlertCommandFromResourceAssemblers
{
    /// <summary>Maps the client severity key to the internal alert level.</summary>
    /// <param name="severity">Client severity key.</param>
    /// <returns>The internal alert level.</returns>
    private static EAlertLevel ToLevel(string? severity) =>
        string.Equals(severity, "critical", StringComparison.OrdinalIgnoreCase)
            ? EAlertLevel.Critical
            : EAlertLevel.Warning;

    /**
     * <summary>
     *     Transforms a creation record into its command. The clinic
     *     identifier stays at the default tenant while the episode
     *     identifier keeps the client string form when numeric (ADR-010)
     * </summary>
     * <param name="resource">The creation record.</param>
     * <returns>The create alert command.</returns>
     */
    public static CreateAlertCommand ToCommandFromResource(CreateAlertResource resource)
    {
        var metric = new AlertMetric(
            resource.VitalSign,
            resource.Value,
            resource.SafeRange);
        return new CreateAlertCommand(
            1,
            int.TryParse(resource.Episode, out var episodeId) ? episodeId : 0,
            resource.Institution,
            resource.PatientName,
            resource.Dni,
            resource.Priority,
            ToLevel(resource.Severity),
            metric);
    }

    /**
     * <summary>
     *     Transforms an update record into its command: the severity
     *     drives the internal state machine
     * </summary>
     * <param name="id">Identifier of the alert.</param>
     * <param name="resource">The update record.</param>
     * <returns>The update alert command.</returns>
     */
    public static UpdateAlertCommand ToCommandFromResource(int id, UpdateAlertResource resource) =>
        new(id, resource.Severity, resource.EscalatedTo);
}
