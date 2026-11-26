using System.Security.Cryptography;
using System.Text;

namespace TriAidPlatform.API.Shared.Infrastructure.Security;

/// <summary>
/// Seals voucher codes so the QR carries an opaque token instead of readable
/// patient data. AES-GCM with a key derived from the API secret: the same key
/// seals on issue and unseals on verification, and nothing else can read it.
/// Output is URL-safe base64 (nonce + ciphertext + tag).
/// </summary>
public class VoucherCodeCipher
{
    private const int NonceSize = 12;
    private const int TagSize = 16;
    private readonly byte[] _key;

    public VoucherCodeCipher(string secret)
    {
        // Derive a 256-bit key from the shared secret with a fixed salt.
        _key = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(secret),
            Encoding.UTF8.GetBytes("triaid-voucher-qr-v1"),
            100_000,
            HashAlgorithmName.SHA256,
            32);
    }

    /// <summary>Seals a plain voucher payload into a URL-safe opaque token.</summary>
    public string Seal(string plain)
    {
        var nonce = RandomNumberGenerator.GetBytes(NonceSize);
        var plainBytes = Encoding.UTF8.GetBytes(plain);
        var cipher = new byte[plainBytes.Length];
        var tag = new byte[TagSize];

        using var gcm = new AesGcm(_key, TagSize);
        gcm.Encrypt(nonce, plainBytes, cipher, tag);

        var output = new byte[nonce.Length + cipher.Length + tag.Length];
        nonce.CopyTo(output, 0);
        cipher.CopyTo(output, nonce.Length);
        tag.CopyTo(output, nonce.Length + cipher.Length);

        return Base64Url.Encode(output);
    }

    /// <summary>Opens a sealed token. Returns null when it is tampered with or from another key.</summary>
    public string? Unseal(string token)
    {
        byte[] raw;
        try { raw = Base64Url.Decode(token); }
        catch { return null; }
        if (raw.Length < NonceSize + TagSize) return null;

        var nonce = raw.AsSpan(0, NonceSize);
        var tag = raw.AsSpan(raw.Length - TagSize, TagSize);
        var cipher = raw.AsSpan(NonceSize, raw.Length - NonceSize - TagSize);
        var plain = new byte[cipher.Length];

        using var gcm = new AesGcm(_key, TagSize);
        try { gcm.Decrypt(nonce, cipher, tag, plain); }
        catch (CryptographicException) { return null; }

        return Encoding.UTF8.GetString(plain);
    }
}
