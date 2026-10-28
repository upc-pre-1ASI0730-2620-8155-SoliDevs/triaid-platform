using TriAidPlatform.API.Shared.Domain.Model;

namespace TriAidPlatform.API.Subscription.Domain.Model;

/**
 * Aggregate root of the Subscription bounded context. Represents the
 * health institution that contracts the platform: every subscription,
 * user and clinical record belongs to exactly one institution
 * (multi-tenant ownership).
 */
public class Institution : IAuditableEntity
{
    /// <summary>Primary key of the institution.</summary>
    public int Id { get; set; }

    /// <summary>Official name of the health institution (unique).</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Tax identification number (RUC in Peru), 11 digits.</summary>
    public string Ruc { get; set; } = string.Empty;

    /// <summary>Physical address of the main facility.</summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>Contact phone number of the institution.</summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>Institutional contact email.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Whether the institution is currently enabled on the platform.</summary>
    public bool Active { get; set; } = true;

    /// <summary>Creation timestamp (auditable).</summary>
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>Last update timestamp (auditable).</summary>
    public DateTimeOffset? UpdatedAt { get; set; }
}
