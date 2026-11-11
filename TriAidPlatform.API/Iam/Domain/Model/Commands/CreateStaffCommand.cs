namespace TriAidPlatform.API.Iam.Domain.Model.Commands;

/**
 * <summary>
 *     The create staff command
 * </summary>
 * <remarks>
 *     This command object includes the profile and credentials of a new
 *     staff account. The account inherits the caller's institution.
 * </remarks>
 */
public record CreateStaffCommand(
    string Username,
    string Password,
    string Names,
    string Surnames,
    string? Email,
    string? Role,
    string? Unit,
    string? Shift);
