using TriAidPlatform.API.Shared.Application.Patterns;
using TriAidPlatform.API.Shared.Domain.Repositories;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Aggregates;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Model;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Commands;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Queries;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Repositories;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Services;
using TriAidPlatform.API.TriageClassification.Domain.Repositories;

namespace TriAidPlatform.API.SpecialtyAssignment.Application.Internal.CommandServices;

/// <summary>
/// Application service of the Specialty Assignment use cases: suggestion
/// from symptom and triage level (US30), referral with queue position
/// (US31, US37), justified reassignment (US32) and demographic rules.
/// </summary>
/// <param name="referralRepository">Repository of the Referral aggregate.</param>
/// <param name="specialtyRepository">Repository of the Specialty catalog.</param>
/// <param name="classificationRepository">Repository of the Classification aggregate (cross-context read).</param>
/// <param name="unitOfWork">Unit of work used to commit the changes.</param>
public class SpecialtyAssignmentService(
    IReferralRepository referralRepository,
    ISpecialtyRepository specialtyRepository,
    IClassificationRepository classificationRepository,
    IUnitOfWork unitOfWork) : ISpecialtyAssignmentService
{
    /// <inheritdoc />
    public async Task<Result<SpecialtySuggestionResult, SpecialtyError>> SuggestAsync(
        SuggestSpecialtyQuery query, CancellationToken cancellationToken = default)
    {
        var suggestion = SpecialtyRules.Suggest(query.Symptom, query.TriageLevel);
        if (!suggestion.HasSuggestion)
            return new Result<SpecialtySuggestionResult, SpecialtyError>.Failure(
                new SpecialtyError(ESpecialtyError.Validation,
                    "The main symptom is required to suggest a specialty."));

        var specialty = await specialtyRepository.FindByKeyAsync(suggestion.SpecialtyKey!, cancellationToken);
        if (specialty is null)
            return new Result<SpecialtySuggestionResult, SpecialtyError>.Failure(
                new SpecialtyError(ESpecialtyError.NotFound,
                    $"Specialty {suggestion.SpecialtyKey} is not part of the catalog."));

        var restriction = !specialty.Admits(query.PatientAge, query.PatientSex);
        return new Result<SpecialtySuggestionResult, SpecialtyError>.Success(
            new SpecialtySuggestionResult(specialty.Key, suggestion.Rule, restriction));
    }

    /// <inheritdoc />
    public async Task<Result<Referral, SpecialtyError>> ReferAsync(
        ReferEpisodeCommand command, string symptom, CancellationToken cancellationToken = default)
    {
        if (await referralRepository.FindByEpisodeIdAsync(command.EpisodeId, cancellationToken) is not null)
            return Failure(ESpecialtyError.Conflict, $"Episode with id {command.EpisodeId} is already referred.");

        var classification = await classificationRepository.FindByEpisodeIdAsync(command.EpisodeId, cancellationToken);
        if (classification is null)
            return Failure(ESpecialtyError.Conflict,
                $"Episode with id {command.EpisodeId} must be classified before referral.");

        var specialty = await specialtyRepository.FindByIdAsync(command.SpecialtyId, cancellationToken);
        if (specialty is null)
            return Failure(ESpecialtyError.NotFound, $"Specialty with id {command.SpecialtyId} was not found.");
        if (!specialty.Active)
            return Failure(ESpecialtyError.Conflict,
                $"Specialty {specialty.Name} is inactive and cannot receive patients.");

        var suggestion = SpecialtyRules.Suggest(symptom, (int)classification.FinalLevel);
        if (suggestion.HasSuggestion &&
            !string.Equals(suggestion.SpecialtyKey, specialty.Key, StringComparison.OrdinalIgnoreCase) &&
            suggestion.Rule == "symptom")
            return Failure(ESpecialtyError.Conflict,
                $"The clinical rules suggest {suggestion.SpecialtyKey} for the registered symptom.");

        var waiting = await referralRepository.CountWaitingBySpecialtyAsync(command.SpecialtyId, cancellationToken);
        var referral = new Referral(command, specialty.Room, waiting + 1);
        await referralRepository.AddAsync(referral, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<Referral, SpecialtyError>.Success(referral);
    }

    /// <inheritdoc />
    public async Task<Result<Referral, SpecialtyError>> ReassignAsync(
        ReassignReferralCommand command, CancellationToken cancellationToken = default)
    {
        var referral = await referralRepository.FindByIdAsync(command.ReferralId, cancellationToken);
        if (referral is null)
            return Failure(ESpecialtyError.NotFound, $"Referral with id {command.ReferralId} was not found.");
        if (!referral.IsWaiting())
            return Failure(ESpecialtyError.Conflict, "An attended referral cannot be reassigned.");

        var specialty = await specialtyRepository.FindByIdAsync(command.NewSpecialtyId, cancellationToken);
        if (specialty is null)
            return Failure(ESpecialtyError.NotFound, $"Specialty with id {command.NewSpecialtyId} was not found.");
        if (!specialty.Active)
            return Failure(ESpecialtyError.Conflict,
                $"Specialty {specialty.Name} is inactive and cannot receive patients.");

        var waiting = await referralRepository.CountWaitingBySpecialtyAsync(command.NewSpecialtyId, cancellationToken);
        try
        {
            referral.Reassign(command.NewSpecialtyId, specialty.Room, waiting + 1, command.Reason);
        }
        catch (InvalidOperationException exception)
        {
            return Failure(ESpecialtyError.Conflict, exception.Message);
        }
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<Referral, SpecialtyError>.Success(referral);
    }

    /// <inheritdoc />
    public async Task<Result<Referral, SpecialtyError>> GetByEpisodeAsync(
        string episodeId, CancellationToken cancellationToken = default)
    {
        var referral = await referralRepository.FindByEpisodeIdAsync(episodeId, cancellationToken);
        return referral is null
            ? Failure(ESpecialtyError.NotFound, $"Episode with id {episodeId} is not referred yet.")
            : new Result<Referral, SpecialtyError>.Success(referral);
    }

    /// <inheritdoc />
    public Task<IEnumerable<Specialty>> ListSpecialtiesAsync(
        bool onlyActive = false, CancellationToken cancellationToken = default) =>
        specialtyRepository.ListAsync(cancellationToken);

    private static Result<Referral, SpecialtyError> Failure(ESpecialtyError type, string message) =>
        new Result<Referral, SpecialtyError>.Failure(new SpecialtyError(type, message));
}
