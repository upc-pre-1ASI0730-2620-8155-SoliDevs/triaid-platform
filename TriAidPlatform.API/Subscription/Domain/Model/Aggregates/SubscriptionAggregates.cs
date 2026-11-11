using TriAidPlatform.API.Shared.Domain.Model;

namespace TriAidPlatform.API.Subscription.Domain.Model.Aggregates;

/// <summary>Commercial plan of the platform catalog.</summary>
public class Plan : IAuditableEntity
{
    /// <summary>Natural identifier (e.g. plan-profesional).</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Display name of the plan.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Audience description of the plan.</summary>
    public string For { get; set; } = string.Empty;

    /// <summary>Monthly price in soles.</summary>
    public decimal MonthlyPrice { get; set; }

    /// <summary>Annual price in soles (discounted).</summary>
    public decimal AnnualPrice { get; set; }

    /// <summary>Staff seats allowed; 0 means unlimited.</summary>
    public int MaxUsers { get; set; }

    /// <summary>Marketing feature list.</summary>
    public List<string> Features { get; set; } = [];

    /// <summary>Creation timestamp (auditable).</summary>
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>Last update timestamp (auditable).</summary>
    public DateTimeOffset? UpdatedAt { get; set; }
}

/// <summary>A charge attempt over a subscription.</summary>
public class Payment : IAuditableEntity
{
    /// <summary>Primary key.</summary>
    public int Id { get; set; }

    /// <summary>Subscription the charge belongs to.</summary>
    public string SubscriptionId { get; set; } = string.Empty;

    /// <summary>Charged amount.</summary>
    public decimal Amount { get; set; }

    /// <summary>Currency code (PEN).</summary>
    public string Currency { get; set; } = "PEN";

    /// <summary>'paid' or 'rejected'.</summary>
    public string Status { get; set; } = "paid";

    /// <summary>Masked card used for the charge.</summary>
    public string Method { get; set; } = string.Empty;

    /// <summary>When the charge was attempted (UTC).</summary>
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;

    /// <summary>Creation timestamp (auditable).</summary>
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>Last update timestamp (auditable).</summary>
    public DateTimeOffset? UpdatedAt { get; set; }
}

/// <summary>
/// Subscription aggregate: the contract between an institution and the
/// platform. Statuses: trial, active, pending (grace), overdue.
/// </summary>
public class Subscription : IAuditableEntity
{
    /// <summary>Natural identifier (e.g. sub-abc123).</summary>
    public string Id { get; set; } = $"sub-{Guid.NewGuid():N}"[..10];

    /// <summary>Tenant institution (owner of the contract).</summary>
    public string Institution { get; set; } = string.Empty;

    /// <summary>Contracted plan identifier.</summary>
    public string PlanId { get; set; } = string.Empty;

    /// <summary>'monthly' or 'annual'.</summary>
    public string BillingCycle { get; set; } = "monthly";

    /// <summary>Current status of the contract.</summary>
    public string Status { get; set; } = "pending";

    /// <summary>When the contract started (UTC).</summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>When the trial period ends (UTC).</summary>
    public DateTime? TrialEndsAt { get; set; }

    /// <summary>Next renewal date (yyyy-MM-dd).</summary>
    public string RenewalDate { get; set; } = string.Empty;

    /// <summary>Grace days before suspension once overdue.</summary>
    public int GraceDays { get; set; } = 7;

    /// <summary>Masked card kept on file for automatic charges.</summary>
    public string PaymentMethod { get; set; } = string.Empty;

    /// <summary>Charge history.</summary>
    public List<Payment> Payments { get; set; } = [];

    /// <summary>Creation timestamp (auditable).</summary>
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>Last update timestamp (auditable).</summary>
    public DateTimeOffset? UpdatedAt { get; set; }
}
