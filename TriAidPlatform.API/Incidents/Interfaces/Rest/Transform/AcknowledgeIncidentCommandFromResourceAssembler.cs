using TriAidPlatform.API.Incidents.Domain.Model.Commands;
using TriAidPlatform.API.Incidents.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.Incidents.Interfaces.Rest.Transform;

/// <summary>
/// Assembler for building an <see cref="AcknowledgeIncidentCommand"/> from route parameters.
/// </summary>
public static class AcknowledgeIncidentCommandFromResourceAssembler
{
    public static AcknowledgeIncidentCommand ToCommandFromResource(int clinicId, int incidentId)
    {
        return new AcknowledgeIncidentCommand(new ClinicId(clinicId), incidentId);
    }
}