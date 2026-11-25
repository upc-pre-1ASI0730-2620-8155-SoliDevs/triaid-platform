using TriAidPlatform.API.Incidents.Domain.Model.Commands;
using TriAidPlatform.API.Incidents.Domain.Model.ValueObjects;
using TriAidPlatform.API.Incidents.Interfaces.Rest.Resources;

namespace TriAidPlatform.API.Incidents.Interfaces.Rest.Transform;

/// <summary>
/// Assembler for building an <see cref="EscalateIncidentCommand"/> from route parameters and request payload.
/// </summary>
public static class EscalateIncidentCommandFromResourceAssembler
{
    public static EscalateIncidentCommand ToCommandFromResource(int clinicId, int alertId, EscalateIncidentResource resource)
    {
        return new EscalateIncidentCommand(new ClinicId(clinicId), alertId, resource.Destination);
    }
}