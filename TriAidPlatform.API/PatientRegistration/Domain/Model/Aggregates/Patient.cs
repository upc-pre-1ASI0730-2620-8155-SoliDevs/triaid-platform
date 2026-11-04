using System.Text.RegularExpressions;
using TriAidPlatform.API.PatientRegistration.Domain.Model.Commands;
using TriAidPlatform.API.PatientRegistration.Domain.Model.ValueObjects;
using TriAidPlatform.API.Shared.Domain.Model;

namespace TriAidPlatform.API.PatientRegistration.Domain.Model.Aggregates;

/// <summary>
/// Aggregate root of the Patient Registration bounded context.
/// Represents a patient registered in the emergency service, either with an
/// official identity document or as an emergency temporary profile.
/// </summary>
public partial class Patient : IAuditableEntity
{
    /// <summary>Prefix used to build emergency temporary codes.</summary>
    private const string TemporaryCodePrefix = "TMP-";

    /// <summary>Unique identifier of the patient.</summary>
    public int Id { get; private set; }

    /// <summary>National identity document number. Null while the profile is temporary.</summary>
    public string? Dni { get; private set; }

    /// <summary>Patient first name.</summary>
    public string FirstName { get; private set; } = string.Empty;

    /// <summary>Patient last name.</summary>
    public string LastName { get; private set; } = string.Empty;

    /// <summary>Patient birth date.</summary>
    public DateOnly BirthDate { get; private set; }

    /// <summary>Patient biological sex.</summary>
    public Sex Sex { get; private set; }

    /// <summary>Contact phone number.</summary>
    public string? Phone { get; private set; }

    /// <summary>Home address.</summary>
    public string? Address { get; private set; }

    /// <summary>Whether the patient was registered without identity document.</summary>
    public bool IsTemporary { get; private set; }

    /// <summary>Emergency identifier assigned to temporary profiles.</summary>
    public string? TemporaryCode { get; private set; }

    /// <summary>Creation timestamp (auditable).</summary>
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>Last update timestamp (auditable).</summary>
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// Protected parameterless constructor strictly required for object materialization.
    /// </summary>
    protected Patient() { }

    /// <summary>
    /// Registers a patient that presents a valid identity document (US06).
    /// </summary>
    /// <param name="command">The registration data.</param>
    /// <exception cref="ArgumentException">Thrown when required data is missing or invalid (US08).</exception>
    public Patient(CreatePatientCommand command)
    {
        EnsureValidDni(command.Dni);
        SetPersonalData(command.FirstName, command.LastName, command.BirthDate, command.Sex);
        Dni = command.Dni;
        Phone = command.Phone;
        Address = command.Address;
        IsTemporary = false;
    }

    /// <summary>
    /// Creates an emergency temporary profile for a patient without identity document (US07).
    /// A unique emergency identifier is assigned to the profile.
    /// </summary>
    /// <param name="command">The basic patient data.</param>
    /// <exception cref="ArgumentException">Thrown when required data is missing or invalid (US08).</exception>
    public Patient(CreateTemporaryProfileCommand command)
    {
        SetPersonalData(command.FirstName, command.LastName, command.BirthDate, command.Sex);
        IsTemporary = true;
        TemporaryCode = GenerateTemporaryCode();
    }

    /// <summary>
    /// Assigns the official identity document to a temporary patient, keeping the
    /// same patient record so the temporary history stays linked (US07, scenario 2).
    /// </summary>
    /// <param name="dni">Valid national identity document number.</param>
    /// <exception cref="InvalidOperationException">Thrown when the patient is not temporary.</exception>
    /// <exception cref="ArgumentException">Thrown when the DNI is invalid.</exception>
    public void RegularizeIdentity(string dni)
    {
        if (!IsTemporary)
            throw new InvalidOperationException("Only temporary patients can regularize their identity.");
        EnsureValidDni(dni);

        Dni = dni;
        IsTemporary = false;
        TemporaryCode = null;
    }

    /// <summary>
    /// Updates the patient contact data.
    /// </summary>
    /// <param name="phone">New contact phone.</param>
    /// <param name="address">New home address.</param>
    public void UpdateContactData(string? phone, string? address)
    {
        Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim();
        Address = string.IsNullOrWhiteSpace(address) ? null : address.Trim();
    }

    /// <summary>
    /// Validates and assigns the mandatory personal data of the patient (US08).
    /// </summary>
    private void SetPersonalData(string firstName, string lastName, DateOnly birthDate, Sex sex)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("The patient first name is required.");
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("The patient last name is required.");
        if (birthDate == default)
            throw new ArgumentException("The patient birth date is required.");
        if (birthDate > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new ArgumentException("The patient birth date cannot be in the future.");
        if (!Enum.IsDefined(sex))
            throw new ArgumentException("The patient sex is not valid.");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        BirthDate = birthDate;
        Sex = sex;
    }

    /// <summary>
    /// Validates that the DNI has exactly 8 digits.
    /// </summary>
    private static void EnsureValidDni(string? dni)
    {
        if (string.IsNullOrWhiteSpace(dni) || !DniRegex().IsMatch(dni))
            throw new ArgumentException("The DNI must have exactly 8 digits.");
    }

    /// <summary>
    /// Builds a 12 character emergency code, for example TMP-4F9A21C0.
    /// </summary>
    private static string GenerateTemporaryCode() =>
        TemporaryCodePrefix + Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();

    [GeneratedRegex(@"^\d{8}$")]
    private static partial Regex DniRegex();
}
