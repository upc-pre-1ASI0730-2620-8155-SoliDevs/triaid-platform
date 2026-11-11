namespace TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Commands;

/// <summary>Command that routes a classified episode to a specialty (US31).</summary>
/// <param name="EpisodeId">Identifier of the classified episode.</param>
/// <param name="SpecialtyId">Identifier of the destination specialty.</param>
public record ReferEpisodeCommand(int EpisodeId, int SpecialtyId);
