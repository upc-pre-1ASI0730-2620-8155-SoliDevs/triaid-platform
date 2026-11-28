namespace TriAidPlatform.API.SpecialtyAssignment.Interfaces.REST.Resources;

/// <summary>
///     Request to deliver a referral voucher by e-mail. <paramref name="AttachToPatient"/> stores the
///     address as the patient's e-mail, so later vouchers go there without asking again.
/// </summary>
/// <param name="Email">Recipient address.</param>
/// <param name="AttachToPatient">Whether the address becomes the patient's e-mail.</param>
public record EmailVoucherResource(string Email, bool AttachToPatient = false);

/// <summary>Outcome of an e-mail delivery.</summary>
/// <param name="Email">Address the voucher was sent to.</param>
/// <param name="Attached">Whether the address was stored on the patient.</param>
public record EmailVoucherSentResource(string Email, bool Attached);
