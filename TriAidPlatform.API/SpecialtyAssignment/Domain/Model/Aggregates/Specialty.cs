using TriAidPlatform.API.Shared.Domain.Model;

namespace TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Aggregates;

/// <summary>
/// Catalog entity of a medical specialty of the institution with its
/// consulting room and demographic admission rules (US30, US32).
/// </summary>
public class Specialty : IAuditableEntity
{
    /// <summary>Unique identifier of the specialty.</summary>
    public int Id { get; private set; }

    /// <summary>Stable key used by the recommendation rules.</summary>
    public string Key { get; private set; } = string.Empty;

    /// <summary>Display name of the specialty.</summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>Consulting room code assigned to the specialty.</summary>
    public string Room { get; private set; } = string.Empty;

    /// <summary>Whether the specialty currently admits patients (US30 - Escenario 2).</summary>
    public bool Active { get; private set; } = true;

    /// <summary>Minimum patient age admitted; null when the specialty is age-open.</summary>
    public int? MinAge { get; private set; }

    /// <summary>Maximum patient age admitted; null when the specialty has no age cap (Pediatrics).</summary>
    public int? MaxAge { get; private set; }

    /// <summary>Required patient sex; null when the specialty admits any (Gynecology).</summary>
    public string? RequiredSex { get; private set; }

    /// <summary>Creation timestamp (auditable).</summary>
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>Last update timestamp (auditable).</summary>
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>Protected parameterless constructor required for object materialization.</summary>
    protected Specialty() { }

    /// <summary>
    /// Creates a catalog specialty with its admission rules (seed and admin).
    /// </summary>
    public Specialty(string key, string name, string room, bool active = true,
        int? minAge = null, int? maxAge = null, string? requiredSex = null)
    {
        Key = key;
        Name = name;
        Room = room;
        Active = active;
        MinAge = minAge;
        MaxAge = maxAge;
        RequiredSex = requiredSex;
    }

    /// <summary>Whether the specialty admits the given demographic profile (US32 - Escenario 2).</summary>
    /// <param name="age">Patient age in years.</param>
    /// <param name="sex">Patient sex code (F/M).</param>
    /// <returns>True when the patient fits the admission rules.</returns>
    public bool Admits(int? age, string? sex)
    {
        if (!Active) return false;
        if (MinAge is not null && age is not null && age < MinAge) return false;
        if (MaxAge is not null && age is not null && age > MaxAge) return false;
        if (RequiredSex is not null && !string.Equals(sex, RequiredSex, StringComparison.OrdinalIgnoreCase)) return false;
        return true;
    }
}
