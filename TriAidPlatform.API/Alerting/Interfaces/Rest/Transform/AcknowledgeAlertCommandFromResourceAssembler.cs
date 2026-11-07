using TriAidPlatform.API.Alerting.Domain.Model.Commands;
using TriAidPlatform.API.Alerting.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.Alerting.Interfaces.Rest.Transform;

/// <summary>
/// Assembler for building an <see cref="AcknowledgeAlertCommand"/> from route parameters.
/// </summary>
public static class AcknowledgeAlertCommandFromResourceAssembler
{
    public static AcknowledgeAlertCommand ToCommandFromResource(int clinicId, int alertId)
    {
        return new AcknowledgeAlertCommand(new ClinicId(clinicId), alertId);
    }
}