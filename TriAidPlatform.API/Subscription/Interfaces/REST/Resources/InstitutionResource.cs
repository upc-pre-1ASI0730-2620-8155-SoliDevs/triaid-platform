namespace TriAidPlatform.API.Subscription.Interfaces.REST.Resources;

/// <summary>Institution data returned by the API.</summary>
/// <param name="Id">Institution identifier.</param>
/// <param name="Name">Official name.</param>
/// <param name="Ruc">Tax identification number.</param>
/// <param name="Address">Main facility address.</param>
/// <param name="Phone">Contact phone.</param>
/// <param name="Email">Contact email.</param>
/// <param name="Active">Whether the institution is enabled.</param>
public record InstitutionResource(
    int Id,
    string Name,
    string Ruc,
    string Address,
    string Phone,
    string Email,
    bool Active);

/// <summary>Payload to register a new institution.</summary>
/// <param name="Name">Official name (required, unique).</param>
/// <param name="Ruc">Tax identification number (required, 11 digits).</param>
/// <param name="Address">Main facility address (optional).</param>
/// <param name="Phone">Contact phone (optional).</param>
/// <param name="Email">Contact email (optional).</param>
public record CreateInstitutionResource(
    string Name,
    string Ruc,
    string Address,
    string Phone,
    string Email);
