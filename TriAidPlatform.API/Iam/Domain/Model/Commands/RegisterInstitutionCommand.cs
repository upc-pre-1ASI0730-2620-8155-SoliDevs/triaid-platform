namespace TriAidPlatform.API.Iam.Domain.Model.Commands;

/**
 * <summary>
 *     The register institution command
 * </summary>
 * <remarks>
 *     This command object includes the institution name, the
 *     administrator profile, the credentials and the card captured for
 *     the trial-end charge. It creates the administrator account, joins
 *     it to the new institution and starts the trial subscription.
 * </remarks>
 */
public record RegisterInstitutionCommand(
    string Institution,
    string Names,
    string Surnames,
    string? Email,
    string Username,
    string Password,
    string? PlanId,
    string? CardNumber);
