namespace TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Commands;

/// <summary>Command that reassigns a referral with justification (US32).</summary>
/// <param name="ReferralId">Identifier of the referral.</param>
/// <param name="NewSpecialtyId">Identifier of the new specialty.</param>
/// <param name="Reason">Mandatory written justification.</param>
public record ReassignReferralCommand(int ReferralId, int NewSpecialtyId, string Reason);
