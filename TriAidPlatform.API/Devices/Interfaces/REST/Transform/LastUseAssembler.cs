using System.Globalization;

namespace TriAidPlatform.API.Devices.Interfaces.REST.Transform;

/// <summary>
/// Translates the last use of a device between the time shown by the frontend (HH:mm)
/// and the date stored in the domain.
/// </summary>
public static class LastUseAssembler
{
    private const string TimeFormat = "HH:mm";

    /// <summary>Converts the frontend last use into a date.</summary>
    /// <param name="lastUse">Time (HH:mm) or ISO 8601 date of the last reading; empty when never used.</param>
    /// <returns>The date of the last reading, or null when the device was never used.</returns>
    /// <exception cref="ArgumentException">Thrown when the value is neither a time nor a date.</exception>
    public static DateTime? ToDateTime(string? lastUse)
    {
        if (string.IsNullOrWhiteSpace(lastUse)) return null;

        var value = lastUse.Trim();
        if (TimeOnly.TryParseExact(value, TimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var time))
            return DateOnly.FromDateTime(DateTime.Today).ToDateTime(time);
        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var date))
            return date;

        throw new ArgumentException($"Invalid last use: '{lastUse}'.");
    }

    /// <summary>Converts the stored last use into the time shown by the frontend.</summary>
    /// <param name="lastUse">Date of the last reading.</param>
    /// <returns>The time (HH:mm), or null when the device was never used.</returns>
    public static string? ToTime(DateTime? lastUse) =>
        lastUse?.ToString(TimeFormat, CultureInfo.InvariantCulture);
}
