using TriAidPlatform.API.Shared.Domain.Model;

namespace TriAidPlatform.API.Iam.Domain.Model.Aggregates;

/// <summary>
/// User aggregate of the IAM bounded context. Every account belongs to
/// exactly one institution (multi-tenant ownership) and carries the role
/// that drives the RBAC rules of the platform.
/// </summary>
public class User : IAuditableEntity
{
    /// <summary>Primary key.</summary>
    public int Id { get; set; }

    /// <summary>Login identifier, unique within the platform.</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>BCrypt hash of the password. Never exposed through the API.</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Given names of the account holder.</summary>
    public string Names { get; set; } = string.Empty;

    /// <summary>Family names of the account holder.</summary>
    public string Surnames { get; set; } = string.Empty;

    /// <summary>Contact email.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Platform role: Triage Administrator, Triage Nurse, Emergency Physician.</summary>
    public string Role { get; set; } = string.Empty;

    /// <summary>Work unit within the institution.</summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>Work shift: Morning, Evening or Night.</summary>
    public string Shift { get; set; } = string.Empty;

    /// <summary>Institution the account belongs to (tenant key).</summary>
    public string Institution { get; set; } = string.Empty;

    /// <summary>Creation timestamp (auditable).</summary>
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>Last update timestamp (auditable).</summary>
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>Whether the account manages the institution (RBAC).</summary>
    public bool IsAdmin => Role.Contains("Admin", StringComparison.OrdinalIgnoreCase);
}
