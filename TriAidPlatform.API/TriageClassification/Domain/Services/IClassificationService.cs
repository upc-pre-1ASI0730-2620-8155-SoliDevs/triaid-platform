using TriAidPlatform.API.Shared.Application.Patterns;
using TriAidPlatform.API.TriageClassification.Domain.Model.Aggregates;
using TriAidPlatform.API.TriageClassification.Domain.Model.ValueObjects;

using TriAidPlatform.API.TriageClassification.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.TriageClassification.Domain.Services;

/// <summary>
/// Contract of the application use cases of the Triage Classification
/// bounded context: suggestion, confirmation and justified modification.
/// </summary>
public interface IClassificationService
{
    /// <summary>Records the NT-158 suggestion for an episode (US19).</summary>
    Task<Result<Classification, ClassificationError>> SuggestAsync(
        int episodeId, VitalSignsSummary summary, CancellationToken cancellationToken = default);

    /// <summary>Approves the suggested level (US20).</summary>
    Task<Result<Classification, ClassificationError>> ConfirmAsync(
        int classificationId, CancellationToken cancellationToken = default);

    /// <summary>Overrides the level with a justification (US21/US22).</summary>
    Task<Result<Classification, ClassificationError>> ModifyAsync(
        int classificationId, ETriageLevel newLevel, string justification,
        CancellationToken cancellationToken = default);

    /// <summary>Retrieves the classification of an episode.</summary>
    Task<Result<Classification, ClassificationError>> GetByEpisodeAsync(
        int episodeId, CancellationToken cancellationToken = default);

    /// <summary>Lists the classifications of a clinic.</summary>
    Task<IEnumerable<Classification>> ListByClinicAsync(
        int clinicId, CancellationToken cancellationToken = default);
}
