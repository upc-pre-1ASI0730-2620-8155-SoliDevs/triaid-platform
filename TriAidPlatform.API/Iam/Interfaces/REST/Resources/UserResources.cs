namespace TriAidPlatform.API.Iam.Interfaces.Rest.Resources;

/**
 * <summary>
 *     The sign in resource
 * </summary>
 * <remarks>
 *     This resource is used to sign in a user
 * </remarks>
 */
public record SignInResource(string Username, string Password);

/**
 * <summary>
 *     The institution registration resource
 * </summary>
 * <remarks>
 *     This resource is used to register an institution together with
 *     its administrator account and the trial subscription (US54).
 * </remarks>
 */
public record InstitutionRegisterResource(
    string Institution,
    string Names,
    string Surnames,
    string? Email,
    string Username,
    string Password,
    string? PlanId,
    string? CardNumber);

/**
 * <summary>
 *     The staff account creation resource
 * </summary>
 * <remarks>
 *     This resource is used by administrators to create staff
 *     accounts under their institution (US55).
 * </remarks>
 */
public record StaffResource(
    string? Username,
    string? Password,
    string? Names,
    string? Surnames,
    string? Email,
    string? Role,
    string? Unit,
    string? Shift);

/**
 * <summary>
 *     The user update resource
 * </summary>
 * <remarks>
 *     Omitted or empty values keep their stored content.
 * </remarks>
 */
public record UpdateUserResource(
    string? Names,
    string? Surnames,
    string? Email,
    string? Role,
    string? Shift,
    string? Unit,
    string? Password);
