namespace TriAidPlatform.API.Devices.Domain.Model.ValueObjects;

/// <summary>
/// Type of medical device or telemetry instrument used in triage.
/// Persisted as the device_type_id column that references the device_types catalog.
/// </summary>
public enum EDeviceType
{
    /// <summary>Pulse oximeter for oxygen saturation and heart rate.</summary>
    PulseOximeter = 1,

    /// <summary>Blood pressure monitor (sphygmomanometer).</summary>
    BloodPressureMonitor = 2,

    /// <summary>Digital clinical thermometer.</summary>
    Thermometer = 3,

    /// <summary>Glucometer for blood glucose levels.</summary>
    Glucometer = 4
}