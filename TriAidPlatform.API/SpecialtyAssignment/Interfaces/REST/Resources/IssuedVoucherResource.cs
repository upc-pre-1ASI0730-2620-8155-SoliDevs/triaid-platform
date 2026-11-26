namespace TriAidPlatform.API.SpecialtyAssignment.Interfaces.REST.Resources;

/// <summary>A voucher code just issued. The token is opaque: only the API can open it.</summary>
/// <param name="ReferralId">Referral the voucher belongs to.</param>
/// <param name="EpisodeId">Episode identifier, for display only.</param>
/// <param name="Token">Sealed, URL-safe token to render as the QR.</param>
public record IssuedVoucherResource(int ReferralId, string EpisodeId, string Token);
