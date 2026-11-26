using TriAidPlatform.API.Incidents.Domain.Model.Aggregates;
using TriAidPlatform.API.Incidents.Domain.Model.Commands;
using TriAidPlatform.API.Shared.Application.Model;

namespace TriAidPlatform.API.Incidents.Application.CommandServices;

/// <summary>
/// Defines the operations that handle commands which create or update incidents.
/// </summary>
public interface IIncidentCommandService
{
    /// <summary>
    /// Creates a new incident and persists it.
    /// </summary>
    /// <param name="command">The incident creation data.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>A result containing the created incident or an application error.</returns>
    Task<Result<Incident>> Handle(CreateIncidentCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the lifecycle state of an existing incident.
    /// </summary>
    /// <param name="command">The incident identifier and requested state update.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>A result containing the updated incident or an application error.</returns>
    Task<Result<Incident>> Handle(UpdateIncidentCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Acknowledges an incident owned by the specified clinic.
    /// </summary>
    /// <param name="command">The incident identifier and owning clinic.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>A result containing the acknowledged incident or an application error.</returns>
    Task<Result<Incident>> Handle(AcknowledgeIncidentCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Escalates an incident owned by the specified clinic.
    /// </summary>
    /// <param name="command">The incident, clinic and escalation destination data.</param>
    /// <param name="cancellationToken">The token used to cancel the operation.</param>
    /// <returns>A result containing the escalated incident or an application error.</returns>
    Task<Result<Incident>> Handle(EscalateIncidentCommand command, CancellationToken cancellationToken = default);
}