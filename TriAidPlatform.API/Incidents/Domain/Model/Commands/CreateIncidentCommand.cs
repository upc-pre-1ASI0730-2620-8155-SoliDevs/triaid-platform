using TriAidPlatform.API.Incidents.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.Incidents.Domain.Model.Commands;

/// <summary>
/// Contains the information required to create a clinical incident for an episode.
/// </summary>
/// <remarks>
/// This command carries the clinic, episode, patient context and triggering vital-sign metric
/// used by the application service to initialize an incident aggregate.
/// </remarks>
/// <param name="IncidentId">The identifier assigned to the incident.</param>
/// <param name="ClinicId">The identifier of the clinic responsible for the alert.</param>
/// <param name="EpisodeId">The identifier of the clinical episode associated with the alert.</param>
/// <param name="Institution">The healthcare institution linked to the episode.</param>
/// <param name="PatientName">The patient's name, if available.</param>
/// <param name="Dni">The patient's identification document number, if available.</param>
/// <param name="Priority">The triage or urgency priority associated with the alert, if available.</param>
/// <param name="Level">The severity level assigned to the alert.</param>
/// <param name="Metric">The vital sign metric that triggered the alert.</param>
public record CreateIncidentCommand(
    int IncidentId,
    int ClinicId, 
    int EpisodeId,
    string Institution,
    string? PatientName,
    string? Dni,
    string? Priority,
    EIncidentLevel Level, 
    IncidentMetric Metric
);