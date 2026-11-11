using TriAidPlatform.API.Iam.Domain.Model.Aggregates;
using TriAidPlatform.API.Iam.Domain.Model.Commands;
using TriAidPlatform.API.Iam.Domain.Model.Queries;
using TriAidPlatform.API.Iam.Interfaces.Rest.Resources;

namespace TriAidPlatform.API.Iam.Interfaces.Rest.Transform;

/**
 * <summary>
 *     The sign in command from resource assembler
 * </summary>
 * <remarks>
 *     This class is used to transform resources into commands
 * </remarks>
 */
public static class SignInCommandFromResourceAssembler
{
    /**
     * <summary>
     *     Transform a sign in resource into a sign in command
     * </summary>
     * <param name="resource">The sign in resource</param>
     * <returns>The sign in command</returns>
     */
    public static SignInCommand ToCommandFromResource(SignInResource resource) =>
        new(resource.Username, resource.Password);
}

/**
 * <summary>
 *     The sign up command from resource assembler
 * </summary>
 */
public static class SignUpCommandFromResourceAssembler
{
    /**
     * <summary>
     *     Transform a username and password into a sign up command
     * </summary>
     * <param name="username">The username</param>
     * <param name="password">The password</param>
     * <returns>The sign up command</returns>
     */
    public static SignUpCommand ToCommandFromResource(string username, string password) =>
        new(username, password);
}

/**
 * <summary>
 *     The register institution command from resource assembler
 * </summary>
 */
public static class RegisterInstitutionCommandFromResourceAssembler
{
    /**
     * <summary>
     *     Transform an institution register resource into its command
     * </summary>
     * <param name="resource">The registration resource</param>
     * <returns>The register institution command</returns>
     */
    public static RegisterInstitutionCommand ToCommandFromResource(InstitutionRegisterResource resource) =>
        new(resource.Institution, resource.Names, resource.Surnames, resource.Email,
            resource.Username, resource.Password, resource.PlanId, resource.CardNumber);
}

/**
 * <summary>
 *     The create staff command from resource assembler
 * </summary>
 */
public static class CreateStaffCommandFromResourceAssembler
{
    /**
     * <summary>
     *     Transform a staff resource into its command
     * </summary>
     * <param name="resource">The staff resource</param>
     * <returns>The create staff command</returns>
     */
    public static CreateStaffCommand ToCommandFromResource(StaffResource resource) =>
        new(resource.Username ?? string.Empty, resource.Password ?? string.Empty,
            resource.Names ?? string.Empty, resource.Surnames ?? string.Empty,
            resource.Email, resource.Role, resource.Unit, resource.Shift);
}

/**
 * <summary>
 *     The update user command from resource assembler
 * </summary>
 */
public static class UpdateUserCommandFromResourceAssembler
{
    /**
     * <summary>
     *     Transform an update resource into its command
     * </summary>
     * <param name="id">The user identifier to update</param>
     * <param name="resource">The update resource</param>
     * <returns>The update user command</returns>
     */
    public static UpdateUserCommand ToCommandFromResource(int id, UpdateUserResource resource) =>
        new(id, resource.Names, resource.Surnames, resource.Email,
            resource.Role, resource.Shift, resource.Unit, resource.Password);
}

/**
 * <summary>
 *     The user resource from entity assembler
 * </summary>
 */
public static class UserResourceFromEntityAssembler
{
    /**
     * <summary>
     *     Convert a user entity into its safe REST representation
     * </summary>
     * <param name="entity">The user entity</param>
     * <returns>The user resource</returns>
     */
    public static UserResource ToResourceFromEntity(User entity) =>
        new(entity.Id, entity.Username, entity.Names, entity.Surnames,
            entity.Email, entity.Role, entity.Unit, entity.Shift, entity.Institution,
            entity.Password);
}

/**
 * <summary>
 *     The authenticated user resource from entity assembler
 * </summary>
 */
public static class AuthenticatedUserResourceFromEntityAssembler
{
    /**
     * <summary>
     *     Convert a user entity and its token into the authenticated resource
     * </summary>
     * <param name="user">The user entity</param>
     * <param name="token">The JWT bearer token</param>
     * <returns>The authenticated user resource</returns>
     */
    public static AuthenticatedUserResource ToResourceFromEntity(User user, string token) =>
        new(UserResourceFromEntityAssembler.ToResourceFromEntity(user), token);
}
