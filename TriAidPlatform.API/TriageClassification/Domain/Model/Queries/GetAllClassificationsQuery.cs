namespace TriAidPlatform.API.TriageClassification.Domain.Model.Queries;

/// <summary>Query that lists the classifications of an institution.</summary>
/// <param name="ClinicId">Identifier of the clinic (tenant).</param>
public record GetAllClassificationsQuery(int ClinicId);
