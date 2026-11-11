using TriAidPlatform.API.Shared.Domain.Model;

namespace TriAidPlatform.API.Iam.Domain.Model.Aggregates;

public partial class User : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
