using TriAidPlatform.API.Iam.Domain.Model.Events;
using TriAidPlatform.API.Iam.Domain.Model.ValueObjects;
using TriAidPlatform.API.Shared.Domain.Model;
using TriAidPlatform.API.Shared.Domain.Model.Aggregates.Internal;

namespace TriAidPlatform.API.Iam.Domain.Model.Aggregates;

/**
 * <summary>
 *     The user aggregate
 * </summary>
 * <remarks>
 *     This class is used to represent a user of a health institution.
 *     Every account belongs to exactly one institution (multi-tenant
 *     ownership) and carries the role that drives the RBAC rules.
 * </remarks>
 */
public partial class User : AggregateRoot
{
    public User() : this(string.Empty, string.Empty)
    {
    }

    public User(string username, string passwordHash)
    {
        Username = username;
        PasswordHash = passwordHash;

        if (!string.IsNullOrWhiteSpace(username))
        {
            AddDomainEvent(new UserRegisteredEvent(Id, username));
        }
    }

    public UserId Id { get; }
    public string Username { get; private set; }

    [System.Text.Json.Serialization.JsonIgnore] public string PasswordHash { get; private set; }

    /// <summary>
    ///     Plain password kept in sync with the hash so the web client
    ///     can validate credentials against the staff listing (ADR-010
    ///     backend swap contract).
    /// </summary>
    public string Password { get; private set; } = string.Empty;

    /// <summary>
    ///     Given names of the account holder.
    /// </summary>
    public string Names { get; private set; } = string.Empty;

    /// <summary>
    ///     Family names of the account holder.
    /// </summary>
    public string Surnames { get; private set; } = string.Empty;

    /// <summary>
    ///     Contact email of the account holder.
    /// </summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    ///     Platform role: Triage Administrator, Triage Nurse, Emergency Physician.
    /// </summary>
    public string Role { get; private set; } = string.Empty;

    /// <summary>
    ///     Work unit within the institution.
    /// </summary>
    public string Unit { get; private set; } = string.Empty;

    /// <summary>
    ///     Work shift: Morning, Evening or Night.
    /// </summary>
    public string Shift { get; private set; } = string.Empty;

    /// <summary>
    ///     Institution the account belongs to (tenant key).
    /// </summary>
    public string Institution { get; private set; } = string.Empty;

    /// <summary>
    ///     Whether the account manages the institution (RBAC).
    /// </summary>
    public bool IsAdmin => Role.Contains("Admin", StringComparison.OrdinalIgnoreCase);

    /**
     * <summary>
     *     Update the username
     * </summary>
     * <param name="username">The new username</param>
     * <returns>The updated user</returns>
     */
    public User UpdateUsername(string username)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        Username = username;
        return this;
    }

    /**
     * <summary>
     *     Update the password hash
     * </summary>
     * <param name="passwordHash">The new password hash</param>
     * <returns>The updated user</returns>
     */
    public User UpdatePasswordHash(string passwordHash)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
        PasswordHash = passwordHash;
        return this;
    }

    /**
     * <summary>
     *     Store the plain password alongside the hash
     * </summary>
     * <param name="password">The plain password</param>
     * <returns>The updated user</returns>
     */
    public User SetPlainPassword(string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        Password = password;
        return this;
    }

    /**
     * <summary>
     *     Update the profile details of the account
     * </summary>
     * <param name="names">The given names</param>
     * <param name="surnames">The family names</param>
     * <param name="email">The contact email</param>
     * <returns>The updated user</returns>
     */
    public User UpdateProfile(string names, string surnames, string email)
    {
        Names = names;
        Surnames = surnames;
        Email = email;
        return this;
    }

    /**
     * <summary>
     *     Update the role assignment of the account (admin operation)
     * </summary>
     * <param name="role">The new platform role</param>
     * <param name="unit">The new work unit</param>
     * <param name="shift">The new work shift</param>
     * <returns>The updated user</returns>
     */
    public User UpdateAssignment(string role, string unit, string shift)
    {
        Role = role;
        Unit = unit;
        Shift = shift;
        return this;
    }

    /**
     * <summary>
     *     Assign the account to an institution. Only intended for the
     *     registration flow where the institution does not exist yet.
     * </summary>
     * <param name="institution">The tenant institution</param>
     * <param name="role">The platform role of the account</param>
     * <returns>The updated user</returns>
     */
    public User JoinInstitution(string institution, string role)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(institution);
        Institution = institution;
        Role = role;
        return this;
    }
}
