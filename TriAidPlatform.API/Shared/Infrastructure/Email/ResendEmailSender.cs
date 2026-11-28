using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TriAidPlatform.API.Shared.Infrastructure.Email;

/// <summary>
/// Sends e-mail through the Resend HTTPS API. The API key comes from the
/// <c>Resend__ApiKey</c> environment variable and the sender from <c>Resend__From</c>.
/// </summary>
public class ResendEmailSender(HttpClient http, IConfiguration configuration, ILogger<ResendEmailSender> logger) : IEmailSender
{
    private const string Endpoint = "https://api.resend.com/emails";
    private readonly string? _apiKey = configuration["Resend:ApiKey"];
    private readonly string _from = configuration["Resend:From"] ?? "Tri-Aid <onboarding@resend.dev>";

    public bool IsConfigured => !string.IsNullOrWhiteSpace(_apiKey);

    public async Task<bool> SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        if (!IsConfigured)
        {
            logger.LogWarning("Resend is not configured (Resend__ApiKey missing); e-mail to {To} not sent", message.To);
            return false;
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, Endpoint);
        request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {_apiKey}");
        request.Content = JsonContent.Create(new
        {
            from = _from,
            to = new[] { message.To },
            subject = message.Subject,
            html = message.Html
        });

        using var response = await http.SendAsync(request, cancellationToken);
        if (response.IsSuccessStatusCode) return true;

        logger.LogWarning("Resend rejected the e-mail to {To}: {Status} {Body}", message.To, (int)response.StatusCode, await response.Content.ReadAsStringAsync(cancellationToken));
        return false;
    }
}
