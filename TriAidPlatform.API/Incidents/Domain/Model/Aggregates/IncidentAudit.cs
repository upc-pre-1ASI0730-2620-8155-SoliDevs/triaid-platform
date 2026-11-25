using TriAidPlatform.API.Shared.Domain.Model;

namespace TriAidPlatform.API.Incidents.Domain.Model.Aggregates;

/// <summary>
/// Adds persistence audit timestamps to the incident aggregate.
/// </summary>
public partial class Incident : IAuditableEntity
{
    /// <summary>
    /// Gets or sets the UTC offset timestamp when the incident was created.
    /// </summary>
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the UTC offset timestamp when the incident was last updated.
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }
}