namespace TriAidPlatform.API.Iam.Domain.Model.Commands;

/**
 * <summary>
 *     The update user command
 * </summary>
 * <remarks>
 *     This command object includes the editable fields of a user
 *     account. Omitted or empty values keep their stored content.
 * </remarks>
 */
public record UpdateUserCommand(
    int Id,
    string? Names,
    string? Surnames,
    string? Email,
    string? Role,
    string? Shift,
    string? Unit,
    string? Password);
