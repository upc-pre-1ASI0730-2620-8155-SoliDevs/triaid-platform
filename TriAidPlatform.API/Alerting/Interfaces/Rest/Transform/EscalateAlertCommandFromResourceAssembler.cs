using TriAidPlatform.API.Alerting.Domain.Model.Commands;
using TriAidPlatform.API.Alerting.Domain.Model.ValueObjects;
using TriAidPlatform.API.Alerting.Interfaces.Rest.Resources;

namespace TriAidPlatform.API.Alerting.Interfaces.Rest.Transform;

/// <summary>
/// Assembler for building an <see cref="EscalateAlertCommand"/> from route parameters and request payload.
/// </summary>
public static class EscalateAlertCommandFromResourceAssembler
{
    public static EscalateAlertCommand ToCommandFromResource(int clinicId, int alertId, EscalateAlertResource resource)
    {
        return new EscalateAlertCommand(new ClinicId(clinicId), alertId, resource.Destination);
    }
}