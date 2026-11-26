using TriAidPlatform.API.Incidents.Application.CommandServices;
using TriAidPlatform.API.Incidents.Domain.Model.Aggregates;
using TriAidPlatform.API.Incidents.Domain.Model.Commands;
using TriAidPlatform.API.Incidents.Domain.Model.Errors;
using TriAidPlatform.API.Incidents.Domain.Repositories;
using TriAidPlatform.API.Shared.Application.Model;
using TriAidPlatform.API.Shared.Domain.Repositories;

namespace TriAidPlatform.API.Incidents.Application.Internal.CommandServices;

/// <summary>
/// Handles commands that create or update <see cref="Incident"/> aggregates.
/// </summary>
/// <param name="incidentRepository">The repository used to access and persist incidents.</param>
/// <param name="unitOfWork">The unit of work used to commit persistence changes.</param>
public class IncidentCommandService(
    IIncidentRepository incidentRepository,
    IUnitOfWork unitOfWork)
    : IIncidentCommandService
{
    /// <summary>
    /// Creates an incident from the supplied command and commits it to the database.
    /// </summary>
    /// <param name="command">The data used to create the incident.</param>
    /// <param name="cancellationToken">The token used to cancel persistence operations.</param>
    /// <returns>The created incident, or a database error.</returns>
    public async Task<Result<Incident>> Handle(CreateIncidentCommand command, CancellationToken cancellationToken = default)
    {
        var incident = new Incident(command);

        try
        {
            await incidentRepository.AddAsync(incident, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Incident>.Success(incident);
        }
        catch (Exception e)
        {
            return Result<Incident>.Failure(IncidentsError.DatabaseError, $"Error creating incident: {e.Message}");
        }
    }

    /// <summary>
    /// Updates an incident state according to the severity supplied by the command.
    /// </summary>
    /// <param name="command">The incident identifier and requested severity update.</param>
    /// <param name="cancellationToken">The token used to cancel persistence operations.</param>
    /// <returns>The updated incident, or an incident-not-found, state-transition or database error.</returns>
    public async Task<Result<Incident>> Handle(UpdateIncidentCommand command, CancellationToken cancellationToken = default)
    {
        var incident = await incidentRepository.GetByIdAsync(command.IncidentId, cancellationToken);
        if (incident is null)
            return Result<Incident>.Failure(IncidentsError.IncidentNotFound, "Incident not found.");

        try
        {
            if (command.Severity == "resolved")
                incident.Acknowledge();
            else if (command.Severity == "escalated")
                incident.Escalate(command.EscalatedTo ?? "Trauma Shock");

            incidentRepository.Update(incident);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Incident>.Success(incident);
        }
        catch (InvalidOperationException e)
        {
            return Result<Incident>.Failure(IncidentsError.InvalidStateTransition, e.Message);
        }
        catch (Exception e)
        {
            return Result<Incident>.Failure(IncidentsError.DatabaseError, $"Error updating incident: {e.Message}");
        }
    }

    /// <summary>
    /// Acknowledges an incident after validating that it belongs to the specified clinic.
    /// </summary>
    /// <param name="command">The incident identifier and owning clinic.</param>
    /// <param name="cancellationToken">The token used to cancel persistence operations.</param>
    /// <returns>The acknowledged incident, or an access, state-transition or database error.</returns>
    public async Task<Result<Incident>> Handle(AcknowledgeIncidentCommand command, CancellationToken cancellationToken = default)
    {
        var incident = await incidentRepository.GetByIdAsync(command.IncidentId, cancellationToken);
        if (incident is null || incident.ClinicId.Value != command.ClinicId.Value)
            return Result<Incident>.Failure(IncidentsError.UnauthorizedClinicAccess, "Incident not found or does not belong to your clinic.");

        try
        {
            incident.Acknowledge();
            incidentRepository.Update(incident);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Incident>.Success(incident);
        }
        catch (InvalidOperationException e)
        {
            return Result<Incident>.Failure(IncidentsError.InvalidStateTransition, e.Message);
        }
        catch (Exception e)
        {
            return Result<Incident>.Failure(IncidentsError.DatabaseError, $"Error acknowledging incident: {e.Message}");
        }
    }

    /// <summary>
    /// Escalates an incident after validating that it belongs to the specified clinic.
    /// </summary>
    /// <param name="command">The incident, clinic and destination escalation data.</param>
    /// <param name="cancellationToken">The token used to cancel persistence operations.</param>
    /// <returns>The escalated incident, or an access, state-transition or database error.</returns>
    public async Task<Result<Incident>> Handle(EscalateIncidentCommand command, CancellationToken cancellationToken = default)
    {
        var incident = await incidentRepository.GetByIdAsync(command.IncidentId, cancellationToken);
        if (incident is null || incident.ClinicId.Value != command.ClinicId.Value)
            return Result<Incident>.Failure(IncidentsError.UnauthorizedClinicAccess, "Incident not found or does not belong to your clinic.");

        try
        {
            incident.Escalate(command.Destination);
            incidentRepository.Update(incident);
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Incident>.Success(incident);
        }
        catch (InvalidOperationException e)
        {
            return Result<Incident>.Failure(IncidentsError.InvalidStateTransition, e.Message);
        }
        catch (Exception e)
        {
            return Result<Incident>.Failure(IncidentsError.DatabaseError, $"Error escalating incident: {e.Message}");
        }
    }
}