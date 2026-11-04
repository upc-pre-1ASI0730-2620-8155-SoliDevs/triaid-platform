using TriAidPlatform.API.PatientRegistration.Domain.Model.Aggregates;
using TriAidPlatform.API.PatientRegistration.Domain.Model.Commands;
using TriAidPlatform.API.PatientRegistration.Domain.Model.ValueObjects;
using TriAidPlatform.API.Shared.Application.Patterns;

namespace TriAidPlatform.API.PatientRegistration.Domain.Services;

/// <summary>
/// Use cases of the triage episode that starts the emergency attention of a patient.
/// </summary>
public interface ITriageEpisodeService
{
    /// <summary>
    /// Opens a new episode for a registered patient.
    /// </summary>
    /// <param name="command">The patient that arrives.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The opened episode or the failure.</returns>
    Task<Result<TriageEpisode, PatientRegistrationError>> OpenAsync(
        OpenTriageEpisodeCommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Moves a waiting episode to triage.
    /// </summary>
    /// <param name="episodeId">Identifier of the episode.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The updated episode or the failure.</returns>
    Task<Result<TriageEpisode, PatientRegistrationError>> StartTriageAsync(
        int episodeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Closes an episode in triage as attended.
    /// </summary>
    /// <param name="episodeId">Identifier of the episode.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The updated episode or the failure.</returns>
    Task<Result<TriageEpisode, PatientRegistrationError>> MarkAttendedAsync(
        int episodeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Closes an episode in triage as referred to a specialty.
    /// </summary>
    /// <param name="episodeId">Identifier of the episode.</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled.</param>
    /// <returns>The updated episode or the failure.</returns>
    Task<Result<TriageEpisode, PatientRegistrationError>> MarkReferredAsync(
        int episodeId, CancellationToken cancellationToken = default);
}
