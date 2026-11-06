namespace TriAidPlatform.API.PatientRegistration.Domain.Model.ValueObjects;

/// <summary>
/// Biological sex of the patient. Persisted as the sex_id column that
/// references the sexes catalog.
/// </summary>
public enum Sex
{
    /// <summary>Male patient.</summary>
    Male = 1,

    /// <summary>Female patient.</summary>
    Female = 2
}
