using TriAidPlatform.API.Incidents.Domain.Model.ValueObjects;
using TriAidPlatform.API.Shared.Domain.Model.Events;

namespace TriAidPlatform.API.Incidents.Domain.Model.Events;

/// <summary>
/// Represents the domain event raised after an incident is successfully created.
/// </summary>
/// <param name="IncidentId">The unique identifier assigned to the created incident.</param>
/// <param name="EpisodeId">The identifier of the clinical episode associated with the incident.</param>
/// <param name="Level">The severity level assigned to the incident.</param>
public record IncidentCreatedEvent(IncidentId IncidentId, int EpisodeId, EIncidentLevel Level) : IEvent;