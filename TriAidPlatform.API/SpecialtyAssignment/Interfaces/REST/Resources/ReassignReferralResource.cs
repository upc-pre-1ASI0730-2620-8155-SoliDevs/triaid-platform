namespace TriAidPlatform.API.SpecialtyAssignment.Interfaces.REST.Resources;

/// <summary>Request body to reassign a referral with justification (US32).</summary>
/// <param name="NewSpecialtyId">Identifier of the new specialty.</param>
/// <param name="Reason">Mandatory written justification.</param>
public record ReassignReferralResource(int NewSpecialtyId, string Reason);
