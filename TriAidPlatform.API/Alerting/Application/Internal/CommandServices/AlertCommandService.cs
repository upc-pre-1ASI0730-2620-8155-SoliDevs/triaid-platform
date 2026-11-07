using TriAidPlatform.API.Alerting.Domain.Model.Aggregates;
using TriAidPlatform.API.Alerting.Domain.Model.Commands;
using TriAidPlatform.API.Alerting.Domain.Repositories;
using TriAidPlatform.API.Shared.Domain.Repositories;

namespace TriAidPlatform.API.Alerting.Application.Internal.CommandServices;

/// <summary>
/// Service responsible for handling commands that mutate the Alert Aggregate Root.
/// Uses Primary Constructors for Dependency Injection.
/// </summary>
/// <param name="alertRepository">The repository interface for Alert data access.</param>
/// <param name="unitOfWork">The unit of work interface to commit database transactions.</param>
public class AlertCommandService(IAlertRepository alertRepository, IUnitOfWork unitOfWork)
{
    /// <summary>
    /// Handles the execution of the <see cref="AcknowledgeAlertCommand"/>.
    /// Enforces multi-tenant data isolation by verifying the ClinicId, mutates the aggregate, and commits the transaction.
    /// </summary>
    /// <param name="command">The command object containing the AlertId and ClinicId.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>An asynchronous task returning the acknowledged <see cref="Alert"/> instance.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the alert does not belong to the requested clinic.</exception>
    /// <exception cref="Exception">Thrown when a database error occurs during the commit phase.</exception>
    public async Task <Alert?> Handle(AcknowledgeAlertCommand command, CancellationToken cancellationToken = default)
    {
       
        var alert = await alertRepository.FindByIdAsync(command.AlertId, cancellationToken);
        
        if (alert == null || alert.ClinicId.Value != command.ClinicId.Value)
        {
            throw new UnauthorizedAccessException("Alert not found or does not belong to your clinic.");
        }
        
        alert.Acknowledge();

        try
        {
            alertRepository.Update(alert);
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new Exception($"An error occurred while acknowledging the alert: {e.Message}");
        }

        return alert;
    }
    /// <summary>
    /// Handles the execution of the <see cref="EscalateAlertCommand"/>.
    /// Validates clinic ownership and escalates the alert to the requested destination.
    /// </summary>
    public async Task<Alert?> Handle(EscalateAlertCommand command, CancellationToken cancellationToken = default)
    {
        var alert = await alertRepository.FindByIdAsync(command.AlertId, cancellationToken);
        
        if (alert == null || alert.ClinicId.Value != command.ClinicId.Value)
        {
            throw new UnauthorizedAccessException("Alert not found or does not belong to your clinic.");
        }

        alert.Escalate(command.Destination);
        
        try
        {
            alertRepository.Update(alert);
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (Exception e)
        {
            throw new Exception($"An error occurred while escalating the alert: {e.Message}");
        }
        
        return alert;
    }
}