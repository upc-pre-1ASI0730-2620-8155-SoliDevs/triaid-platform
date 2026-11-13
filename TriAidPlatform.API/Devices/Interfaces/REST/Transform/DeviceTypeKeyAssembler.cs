using TriAidPlatform.API.Devices.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.Devices.Interfaces.REST.Transform;

/// <summary>
/// Translates between the device type keys used by the frontend (pa, spo2, fc, temp)
/// and the <see cref="EDeviceType"/> values of the domain.
/// </summary>
public static class DeviceTypeKeyAssembler
{
    private static readonly Dictionary<string, EDeviceType> TypeKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        ["pa"] = EDeviceType.BloodPressureMonitor,
        ["spo2"] = EDeviceType.PulseOximeter,
        ["fc"] = EDeviceType.HeartRateMonitor,
        ["temp"] = EDeviceType.Thermometer
    };

    /// <summary>Resolves a device type from its frontend key or its enum name.</summary>
    /// <param name="type">Type key (pa, spo2, fc, temp) or enum name.</param>
    /// <returns>The device type.</returns>
    /// <exception cref="ArgumentException">Thrown when the type is not recognized.</exception>
    public static EDeviceType ToDeviceType(string type)
    {
        if (!string.IsNullOrWhiteSpace(type) && TypeKeys.TryGetValue(type.Trim(), out var keyType))
            return keyType;
        if (Enum.TryParse<EDeviceType>(type, true, out var namedType) && Enum.IsDefined(namedType))
            return namedType;

        throw new ArgumentException($"Invalid device type: '{type}'.");
    }

    /// <summary>Gets the frontend key of a device type, or its enum name when it has no key.</summary>
    /// <param name="type">Device type.</param>
    /// <returns>The type key.</returns>
    public static string ToTypeKey(EDeviceType type) =>
        TypeKeys.FirstOrDefault(entry => entry.Value == type).Key ?? type.ToString();
}
