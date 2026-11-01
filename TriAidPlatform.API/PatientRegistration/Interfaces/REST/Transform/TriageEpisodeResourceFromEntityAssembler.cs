using TriAidPlatform.API.PatientRegistration.Domain.Model.Aggregates;
using TriAidPlatform.API.PatientRegistration.Domain.Model.Commands;
using TriAidPlatform.API.PatientRegistration.Interfaces.REST.Resources;

namespace TriAidPlatform.API.PatientRegistration.Interfaces.REST.Transform;

/// <summary>Maps between TriageEpisode entities, commands and REST resources.</summary>
public static class TriageEpisodeResourceFromEntityAssembler
{
    /// <summary>Converts an entity into its REST representation.</summary>
    public static TriageEpisodeResource ToResourceFromEntity(TriageEpisode entity) =>
        new(entity.Id, entity.PatientId, entity.ArrivalTime, entity.State.ToString());

    /// <summary>Converts an open episode payload into a command.</summary>
    public static OpenTriageEpisodeCommand ToCommandFromResource(OpenTriageEpisodeResource resource) =>
        new(resource.PatientId);
}
