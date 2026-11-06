using TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Aggregates;

namespace TriAidPlatform.API.SpecialtyAssignment.Interfaces.REST.Resources;

/// <summary>REST representation of a catalog specialty.</summary>
/// <param name="Id">Identifier of the specialty.</param>
/// <param name="Key">Stable key used by the rules.</param>
/// <param name="Name">Display name.</param>
/// <param name="Room">Consulting room code.</param>
/// <param name="Active">Whether the specialty admits patients.</param>
/// <param name="MinAge">Minimum admitted age.</param>
/// <param name="MaxAge">Maximum admitted age.</param>
/// <param name="RequiredSex">Required patient sex.</param>
public record SpecialtyResource(int Id, string Key, string Name, string Room, bool Active, int? MinAge, int? MaxAge, string? RequiredSex)
{
    /// <summary>Maps the catalog entity to its REST representation.</summary>
    public static SpecialtyResource FromEntity(Specialty specialty) => new(
        specialty.Id, specialty.Key, specialty.Name, specialty.Room,
        specialty.Active, specialty.MinAge, specialty.MaxAge, specialty.RequiredSex);
}
