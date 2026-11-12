using TriAidPlatform.API.TriageClassification.Domain.Model.Aggregates;
using TriAidPlatform.API.TriageClassification.Domain.Repositories;
using TriAidPlatform.API.TriageClassification.Domain.Services;

namespace TriAidPlatform.API.TriageClassification.Application.Internal.QueryServices;

/// <summary>Application service of the classification read use cases.</summary>
/// <param name="classificationRepository">Repository of the Classification aggregate.</param>
public class ClassificationQueryService(IClassificationRepository classificationRepository)
{
    /// <summary>Retrieves the classification of an episode.</summary>
    public Task<Classification?> GetByEpisodeAsync(string episodeId, CancellationToken cancellationToken = default) =>
        classificationRepository.FindByEpisodeIdAsync(episodeId, cancellationToken);

    /// <summary>Lists the classifications of a clinic.</summary>
    public Task<IEnumerable<Classification>> ListByClinicAsync(int clinicId, CancellationToken cancellationToken = default) =>
        classificationRepository.FindByClinicIdAsync(clinicId, cancellationToken);
}
