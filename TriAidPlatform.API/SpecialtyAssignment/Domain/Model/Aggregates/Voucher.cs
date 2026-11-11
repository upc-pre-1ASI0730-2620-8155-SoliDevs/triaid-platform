namespace TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Aggregates;

/**
 * <summary>
 *     Voucher of a referred patient: proof of the referral sent to the
 *     patient portal or front desk, with the QR code payload (ADR-010
 *     web client contract).
 * </summary>
 */
public class Voucher
{
    /// <summary>Unique identifier of the voucher.</summary>
    public int Id { get; set; }

    /// <summary>Identifier of the referral the voucher proves.</summary>
    public int ReferralId { get; set; }

    /// <summary>QR payload with the referral summary.</summary>
    public string QrCode { get; set; } = string.Empty;

    /// <summary>Delivery channel of the voucher (QrPortal, Print, WhatsApp).</summary>
    public string Channel { get; set; } = string.Empty;

    /// <summary>UTC timestamp when the voucher was sent.</summary>
    public DateTime? SentAt { get; set; }

    /// <summary>Institution the voucher belongs to.</summary>
    public string Institution { get; set; } = string.Empty;
}

/**
 * <summary>
 *     Voucher body used by the web client save flows, tolerant of a
 *     null identifier on creation (ADR-010).
 * </summary>
 * <param name="ReferralId">Identifier of the referral the voucher proves.</param>
 * <param name="QrCode">QR payload with the referral summary.</param>
 * <param name="Channel">Delivery channel of the voucher.</param>
 * <param name="SentAt">UTC timestamp when the voucher was sent.</param>
 * <param name="Institution">Institution the voucher belongs to.</param>
 */
public record SaveVoucherResource(
    int ReferralId,
    string QrCode,
    string Channel,
    DateTime? SentAt,
    string Institution);
