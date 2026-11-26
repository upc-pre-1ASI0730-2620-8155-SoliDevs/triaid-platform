using TriAidPlatform.API.Shared.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.Incidents.Domain.Model.ValueObjects;

/// <summary>
/// Represents the strongly typed unique identifier of an incident.
/// </summary>
/// <param name="Value">The underlying integer identifier.</param>
public readonly record struct IncidentId(int Value) : IEntityId<int>
{
    /// <summary>
    /// Converts an incident identifier to its underlying integer value.
    /// </summary>
    /// <param name="id">The incident identifier to convert.</param>
    public static implicit operator int(IncidentId id) => id.Value;

    /// <summary>
    /// Converts an integer value to an incident identifier.
    /// </summary>
    /// <param name="value">The underlying integer identifier.</param>
    public static implicit operator IncidentId(int value) => new(value);

    /// <summary>
    /// Returns the incident identifier as its underlying integer text.
    /// </summary>
    /// <returns>The string representation of <see cref="Value"/>.</returns>
    public override string ToString() => Value.ToString();
}