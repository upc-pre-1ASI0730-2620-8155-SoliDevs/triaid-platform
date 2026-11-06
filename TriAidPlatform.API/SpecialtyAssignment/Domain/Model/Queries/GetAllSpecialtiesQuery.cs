namespace TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Queries;

/// <summary>Query that lists the specialty catalog.</summary>
/// <param name="OnlyActive">When true, only active specialties are returned.</param>
public record GetAllSpecialtiesQuery(bool OnlyActive = false);
