namespace TriAidPlatform.API.Iam.Interfaces.REST.Resources;

/// <summary>Sign-in payload.</summary>
/// <param name="Username">Account username.</param>
/// <param name="Password">Plain-text password (hashed server-side).</param>
public record LoginResource(string Username, string Password);

/// <summary>Institution self-service registration payload (US54).</summary>
/// <param name="Institution">Official institution name.</param>
/// <param name="Names">Administrator given names.</param>
/// <param name="Surnames">Administrator family names.</param>
/// <param name="Email">Administrator contact email.</param>
/// <param name="Username">Administrator username.</param>
/// <param name="Password">Administrator password.</param>
/// <param name="PlanId">Contracted plan identifier.</param>
/// <param name="CardNumber">Card captured for the trial-end charge.</param>
public record RegisterResource(
    string Institution, string Names, string Surnames,
    string? Email, string Username, string Password,
    string? PlanId, string? CardNumber);

/// <summary>Authenticated user projection (never includes credentials).</summary>
/// <param name="Id">User identifier.</param>
/// <param name="Username">Login identifier.</param>
/// <param name="Names">Given names.</param>
/// <param name="Surnames">Family names.</param>
/// <param name="Email">Contact email.</param>
/// <param name="Role">Platform role.</param>
/// <param name="Unit">Work unit.</param>
/// <param name="Shift">Work shift.</param>
/// <param name="Institution">Tenant institution.</param>
public record UserResource(
    int Id, string Username, string Names, string Surnames,
    string Email, string Role, string Unit, string Shift, string Institution);

/// <summary>Staff account creation payload (admin-only, US55).</summary>
/// <param name="Username">Login identifier.</param>
/// <param name="Password">Initial password.</param>
/// <param name="Names">Given names.</param>
/// <param name="Surnames">Family names.</param>
/// <param name="Email">Contact email.</param>
/// <param name="Role">Platform role.</param>
/// <param name="Unit">Work unit.</param>
/// <param name="Shift">Work shift.</param>
public record StaffResource(
    string? Username, string? Password, string? Names, string? Surnames,
    string? Email, string? Role, string? Unit, string? Shift);

/// <summary>Profile update payload; omitted or empty fields keep their value.</summary>
/// <param name="Names">Given names.</param>
/// <param name="Surnames">Family names.</param>
/// <param name="Email">Contact email.</param>
/// <param name="Role">Platform role (admin edits only).</param>
/// <param name="Shift">Work shift (admin edits only).</param>
/// <param name="Unit">Work unit (admin edits only).</param>
/// <param name="Password">New password (optional).</param>
public record UpdateUserResource(
    string? Names, string? Surnames, string? Email,
    string? Role, string? Shift, string? Unit, string? Password);
