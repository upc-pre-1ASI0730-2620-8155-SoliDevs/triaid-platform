namespace TriAidPlatform.API.Shared.Infrastructure.Email;

/// <summary>A request to send a transactional e-mail.</summary>
public record EmailMessage(string To, string Subject, string Html);

/// <summary>Delivers transactional e-mail. Implementations read their credentials from configuration, never from code.</summary>
public interface IEmailSender
{
    /// <returns>True when the provider accepted the message.</returns>
    Task<bool> SendAsync(EmailMessage message, CancellationToken cancellationToken = default);

    /// <summary>True when the sender is configured (credentials present).</summary>
    bool IsConfigured { get; }
}
