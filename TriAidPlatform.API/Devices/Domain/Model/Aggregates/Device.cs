using TriAidPlatform.API.Devices.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.Devices.Domain.Model.Aggregates;

/// <summary>
/// Represents a medical device or telemetry instrument in the system.
/// Acts as an aggregate root for device management and telemetry capture.
/// </summary>
public class Device
{
    public int Id { get; }
    public string? SerialNumber { get; private set; }
    public EDeviceType Type { get; private set; }
    public string Model { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime RegisteredAt { get; set; }

    // Required by Entity Framework Core
    protected Device()
    {
        Model = string.Empty;
        IsActive = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Device"/> class.
    /// </summary>
    /// <param name="serialNumber">The unique serial number of the hardware, when it is known.</param>
    /// <param name="type">The type of medical device (PulseOximeter, BloodPressureMonitor, etc.).</param>
    /// <param name="model">The commercial model name or description.</param>
    public Device(string? serialNumber, EDeviceType type, string model)
    {
        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("The model cannot be null or empty.", nameof(model));

        SerialNumber = string.IsNullOrWhiteSpace(serialNumber) ? null : serialNumber.Trim();
        Type = type;
        Model = model.Trim();
        IsActive = true;
        RegisteredAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the device information.
    /// </summary>
    public void UpdateDetails(string? serialNumber, EDeviceType type, string model)
    {
        if (string.IsNullOrWhiteSpace(model))
            throw new ArgumentException("The model cannot be null or empty.", nameof(model));

        SerialNumber = string.IsNullOrWhiteSpace(serialNumber) ? null : serialNumber.Trim();
        Type = type;
        Model = model.Trim();
    }

    /// <summary>
    /// Deactivates the device, making it unavailable for triage measurements.
    /// </summary>
    public void Deactivate() => IsActive = false;

    /// <summary>
    /// Activates the device.
    /// </summary>
    public void Activate() => IsActive = true;
}