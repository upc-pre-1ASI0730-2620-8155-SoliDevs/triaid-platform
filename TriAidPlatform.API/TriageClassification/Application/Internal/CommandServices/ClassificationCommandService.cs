using TriAidPlatform.API.PatientRegistration.Domain.Model.ValueObjects;
using TriAidPlatform.API.PatientRegistration.Domain.Repositories;
using TriAidPlatform.API.Shared.Application.Patterns;
using TriAidPlatform.API.Shared.Domain.Repositories;
using TriAidPlatform.API.TriageClassification.Domain.Model.Aggregates;
using TriAidPlatform.API.TriageClassification.Domain.Model.Commands;
using TriAidPlatform.API.TriageClassification.Domain.Model.ValueObjects;
using TriAidPlatform.API.TriageClassification.Domain.Repositories;
using TriAidPlatform.API.TriageClassification.Domain.Services;

namespace TriAidPlatform.API.TriageClassification.Application.Internal.CommandServices;

/// <summary>
/// Application service of the classification write use cases. Delegates the
/// rule evaluation to the NT-158 engine and the lifecycle rules to the
/// Classification aggregate.
/// </summary>
/// <param name="classificationRepository">Repository of the Classification aggregate.</param>
/// <param name="unitOfWork">Unit of work used to commit the changes.</param>
public class ClassificationCommandService(
    IClassificationRepository classificationRepository,
    IUnitOfWork unitOfWork) : IClassificationService
{
    /// <inheritdoc />
    public async Task<Result<Classification, ClassificationError>> SuggestAsync(
        string episodeId, VitalSignsSummary summary, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(episodeId))
            return Failure(EClassificationError.NotFound, "The episode identifier is required.");

        if (await classificationRepository.FindByEpisodeIdAsync(episodeId, cancellationToken) is not null)
            return Failure(EClassificationError.Conflict, $"Episode with id {episodeId} is already classified.");

        var evaluation = NT158Engine.Evaluate(summary);
        var classification = new Classification(
            new SuggestClassificationCommand(episodeId, evaluation.Level ?? ETriageLevel.LessUrgent, evaluation.MissingMetrics.ToArray()));
        await classificationRepository.AddAsync(classification, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<Classification, ClassificationError>.Success(classification);
    }

    /// <inheritdoc />
    public async Task<Result<Classification, ClassificationError>> ConfirmAsync(
        int classificationId, CancellationToken cancellationToken = default)
    {
        var classification = await classificationRepository.FindByIdAsync(classificationId, cancellationToken);
        if (classification is null)
            return Failure(EClassificationError.NotFound, $"Classification with id {classificationId} was not found.");
        try
        {
            classification.Confirm();
        }
        catch (InvalidOperationException exception)
        {
            return Failure(EClassificationError.Conflict, exception.Message);
        }
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<Classification, ClassificationError>.Success(classification);
    }

    /// <inheritdoc />
    public async Task<Result<Classification, ClassificationError>> ModifyAsync(
        int classificationId, ETriageLevel newLevel, string justification,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(justification))
            return Failure(EClassificationError.Validation,
                "A written justification is required to change the suggested level.");

        var classification = await classificationRepository.FindByIdAsync(classificationId, cancellationToken);
        if (classification is null)
            return Failure(EClassificationError.NotFound, $"Classification with id {classificationId} was not found.");
        try
        {
            classification.Modify(newLevel, justification);
        }
        catch (InvalidOperationException exception)
        {
            return Failure(EClassificationError.Conflict, exception.Message);
        }
        await unitOfWork.CompleteAsync(cancellationToken);
        return new Result<Classification, ClassificationError>.Success(classification);
    }

    /// <inheritdoc />
    public async Task<Result<Classification, ClassificationError>> GetByEpisodeAsync(
        string episodeId, CancellationToken cancellationToken = default)
    {
        var classification = await classificationRepository.FindByEpisodeIdAsync(episodeId, cancellationToken);
        return classification is null
            ? Failure(EClassificationError.NotFound, $"Episode with id {episodeId} is not classified yet.")
            : new Result<Classification, ClassificationError>.Success(classification);
    }

    /// <inheritdoc />
    public Task<IEnumerable<Classification>> ListByClinicAsync(
        int clinicId, CancellationToken cancellationToken = default) =>
        classificationRepository.FindByClinicIdAsync(clinicId, cancellationToken);

    private static Result<Classification, ClassificationError> Failure(EClassificationError type, string message) =>
        new Result<Classification, ClassificationError>.Failure(new ClassificationError(type, message));
}
