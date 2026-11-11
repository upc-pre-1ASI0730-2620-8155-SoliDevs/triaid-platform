namespace TriAidPlatform.API.Iam.Domain.Model.Queries;

/**
 * <summary>
 *     The get users by institution query
 * </summary>
 * <remarks>
 *     This query object is used to get the staff accounts of a tenant
 *     institution (multi-tenant scoping).
 * </remarks>
 */
public record GetUsersByInstitutionQuery(string Institution);
