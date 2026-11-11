namespace TriAidPlatform.API.Iam.Interfaces.Rest.Resources;

/**
 * <summary>
 *     The open session resource
 * </summary>
 * <remarks>
 *     Sent by the web client right after credentials are matched
 *     against the staff listing.
 * </remarks>
 */
public record OpenSessionResource(int UserId, string Username, string OpenedAt);

/**
 * <summary>
 *     The session resource
 * </summary>
 * <remarks>
 *     Server representation of an open web session.
 * </remarks>
 */
public record SessionResource(int Id, int UserId, string Username, string OpenedAt);
