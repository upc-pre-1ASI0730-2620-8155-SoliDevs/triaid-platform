namespace TriAidPlatform.API.Shared.Infrastructure.Security;

/// <summary>URL-safe base64 (RFC 4648 §5) without padding, safe in a QR and in a path segment.</summary>
public static class Base64Url
{
    public static string Encode(byte[] bytes) =>
        Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    public static byte[] Decode(string text) =>
        Convert.FromBase64String(text.Replace('-', '+').Replace('_', '/').PadRight(text.Length + (4 - text.Length % 4) % 4, '='));
}
