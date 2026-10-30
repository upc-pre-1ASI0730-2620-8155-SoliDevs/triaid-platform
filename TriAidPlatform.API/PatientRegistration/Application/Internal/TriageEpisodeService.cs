using TriAidPlatform.API.PatientRegistration.Domain.Model.Aggregates;
using TriAidPlatform.API.PatientRegistration.Domain.Model.Commands;
using TriAidPlatform.API.PatientRegistration.Domain.Model.ValueObjects;
using TriAidPlatform.API.PatientRegistration.Domain.Repositories;
using TriAidPlatform.API.PatientRegistration.Domain.Services;
using TriAidPlatform.API.Shared.Application.Patterns;
using TriAidPlatform.API.Shared.Domain.Repositories;

namespace TriAidPlatform.API.PatientRegistration.Application.Internal;

/// <summary>
/// Application service that orchestrates the triage episode use cases.
/// State rules live in the <see cref="TriageEpisode"/> aggregate; this service
/// checks the patient, avoids duplicated open episodes and persists the changes.
/// </summary>
/// <param name="episodeRepository">Repository of the TriageEpisode aggregate.</param>
/// <param name="patientRepository">Repository of the Patient aggregate.</param>
/// <param name="unitOfWork">Unit of work used to commit the changes.</param>
public class TriageEpisodeService(
    ITriageEpisodeRepository episodeRepository,
    IPatientRepository patientRepository,
    IUnitOfWork unitOfWork) : ITriageEpisodeService
{
    /// <inheritdoc />
    public async Task<Result<TriageEpisode, PatientRegistrationError>> OpenAsync(
        OpenTriageEpisodeCommand command, CancellationToken cancellationToken = default)
    {
        if (await patientRepository.FindByIdAsync(command.PatientId, cancellationToken) is null)
            return Failure(EPatientRegistrationErrorType.NotFound,
                $"Patient with id {command.PatientId} was not found.");

        if (await episodeRepository.ExistsOpenByPatientIdAsync(command.PatientId, cancellationToken))
            return Failure(EPatientRegistrationErrorType.Conflict,
                $"Patient with id {command.PatientId} already has an episode in progress.");

        var episode = new TriageEpisode(command);
        await episodeRepository.AddAsync(episode, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<TriageEpisode, PatientRegistrationError>.Success(episode);
    }

    /// <inheritdoc />
    public Task<Result<TriageEpisode, PatientRegistrationError>> StartTriageAsync(
        int episodeId, CancellationToken cancellationToken = default) =>
        ChangeStateAsync(episodeId, episode => episode.StartTriage(), cancellationToken);

    /// <inheritdoc />
    public Task<Result<TriageEpisode, PatientRegistrationError>> MarkAttendedAsync(
        int episodeId, CancellationToken cancellationToken = default) =>
        ChangeStateAsync(episodeId, episode => episode.MarkAttended(), cancellationToken);

    /// <inheritdoc />
    public Task<Result<TriageEpisode, PatientRegistrationError>> MarkReferredAsync(
        int episodeId, CancellationToken cancellationToken = default) =>
        ChangeStateAsync(episodeId, episode => episode.MarkReferred(), cancellationToken);

    /// <summary>
    /// Loads the episode, applies the state transition and persists it.
    /// </summary>
    private async Task<Result<TriageEpisode, PatientRegistrationError>> ChangeStateAsync(
        int episodeId, Action<TriageEpisode> transition, CancellationToken cancellationToken)
    {
        var episode = await episodeRepository.FindByIdAsync(episodeId, cancellationToken);
        if (episode is null)
            return Failure(EPatientRegistrationErrorType.NotFound, $"Triage episode with id {episodeId} was not found.");

        try
        {
            transition(episode);
        }
        catch (InvalidOperationException e)
        {
            return Failure(EPatientRegistrationErrorType.Validation, e.Message);
        }

        episodeRepository.Update(episode);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<TriageEpisode, PatientRegistrationError>.Success(episode);
    }

    private static Result<TriageEpisode, PatientRegistrationError> Failure(
        EPatientRegistrationErrorType type, string message) =>
        new Result<TriageEpisode, PatientRegistrationError>.Failure(new PatientRegistrationError(type, message));
}
