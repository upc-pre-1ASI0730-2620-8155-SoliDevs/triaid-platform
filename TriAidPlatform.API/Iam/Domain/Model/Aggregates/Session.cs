namespace TriAidPlatform.API.Iam.Domain.Model.Aggregates;

/// <summary>
/// Open session of a signed-in user. The session id travels as the
/// Bearer token of subsequent requests.
/// </summary>
public class Session
{
    /// <summary>Opaque token used as the Bearer credential.</summary>
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>Identifier of the signed-in user.</summary>
    public int UserId { get; set; }

    /// <summary>Username captured for audit purposes.</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>When the session was opened (UTC).</summary>
    public DateTimeOffset OpenedAt { get; set; } = DateTimeOffset.UtcNow;
}
