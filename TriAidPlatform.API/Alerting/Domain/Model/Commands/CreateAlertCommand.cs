using TriAidPlatform.API.Alerting.Domain.Model.Aggregates;
using TriAidPlatform.API.Alerting.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.Alerting.Domain.Model.Commands;

/// <summary>
/// Command containing the information required to create a clinical alert for an episode.
/// </summary>
/// <param name="ClinicId">The identifier of the clinic responsible for the alert.</param>
/// <param name="EpisodeId">The identifier of the clinical episode associated with the alert.</param>
/// <param name="Institution">The healthcare institution linked to the episode.</param>
/// <param name="PatientName">The patient's name, if available.</param>
/// <param name="Dni">The patient's identification document number, if available.</param>
/// <param name="Priority">The triage or urgency priority associated with the alert, if available.</param>
/// <param name="Level">The severity level assigned to the alert.</param>
/// <param name="Metric">The vital sign metric that triggered the alert.</param>
public record CreateAlertCommand(
    int ClinicId, 
    int EpisodeId,
    string Institution,
    string? PatientName,
    string? Dni,
    string? Priority,
    EAlertLevel Level, 
    AlertMetric Metric
);