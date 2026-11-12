using TriAidPlatform.API.Devices.Domain.Model.Commands;
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

    /// <summary>Whether the device is online and able to send readings.</summary>
    public bool Online { get; private set; }

    /// <summary>Moment of the last reading taken with the device, if any.</summary>
    public DateTime? LastUse { get; private set; }

    /// <summary>Identifier of the triage episode the device is linked to (e.g. EP-261105-0001).</summary>
    public string? LinkedEpisodeId { get; private set; }

    /// <summary>Moment the device was linked to its current triage episode.</summary>
    public DateTime? LinkedAt { get; private set; }

    /// <summary>Institution that owns the device.</summary>
    public string Institution { get; private set; }

    // Required by Entity Framework Core
    protected Device()
    {
        Model = string.Empty;
        Institution = string.Empty;
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
        Institution = string.Empty;
    }

    /// <summary>
    /// Registers a device from the <see cref="CreateDeviceCommand"/>.
    /// </summary>
    /// <param name="command">Data of the device to register.</param>
    public Device(CreateDeviceCommand command) : this(command.SerialNumber, command.Type, command.Model)
    {
        SetOnline(command.Online);
        RegisterUse(command.LastUse);
        AssignInstitution(command.Institution);
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
    /// Replaces the full record of the device with the <see cref="UpdateDeviceCommand"/>.
    /// </summary>
    /// <param name="command">New state of the device.</param>
    /// <exception cref="ArgumentException">Thrown when the model is empty.</exception>
    public void Update(UpdateDeviceCommand command)
    {
        UpdateDetails(command.SerialNumber, command.Type, command.Model);
        SetOnline(command.Online);
        RegisterUse(command.LastUse);
        AssignInstitution(command.Institution);
        if (string.IsNullOrWhiteSpace(command.LinkedEpisodeId)) ReleaseFromEpisode();
        else LinkToEpisode(command.LinkedEpisodeId, command.LinkedAt);
    }

    /// <summary>
    /// Deactivates the device, making it unavailable for triage measurements.
    /// </summary>
    public void Deactivate() => IsActive = false;

    /// <summary>
    /// Activates the device.
    /// </summary>
    public void Activate() => IsActive = true;

    /// <summary>
    /// Sets whether the device is online and able to send readings.
    /// </summary>
    /// <param name="online">True when the device is online.</param>
    public void SetOnline(bool online) => Online = online;

    /// <summary>
    /// Records the moment of the last reading taken with the device.
    /// </summary>
    /// <param name="lastUse">Moment of the last reading, or null when the device was never used.</param>
    public void RegisterUse(DateTime? lastUse) => LastUse = lastUse;

    /// <summary>
    /// Links the device to a triage episode so it monitors that patient.
    /// The link time is kept while the device stays on the same episode.
    /// </summary>
    /// <param name="episodeId">Identifier of the triage episode.</param>
    /// <param name="linkedAt">Moment of the link, or null to stamp it now.</param>
    /// <exception cref="ArgumentException">Thrown when the episode identifier is empty.</exception>
    public void LinkToEpisode(string episodeId, DateTime? linkedAt = null)
    {
        if (string.IsNullOrWhiteSpace(episodeId))
            throw new ArgumentException("The episode ID cannot be null or empty.", nameof(episodeId));

        var id = episodeId.Trim();
        LinkedAt = linkedAt ?? (LinkedEpisodeId == id && LinkedAt is not null ? LinkedAt : DateTime.UtcNow);
        LinkedEpisodeId = id;
    }

    /// <summary>
    /// Releases the device from its triage episode, making it available for the next patient.
    /// </summary>
    public void ReleaseFromEpisode()
    {
        LinkedEpisodeId = null;
        LinkedAt = null;
    }

    /// <summary>
    /// Tags the device with the institution that owns it.
    /// </summary>
    /// <param name="institution">Name of the institution.</param>
    public void AssignInstitution(string? institution) =>
        Institution = institution?.Trim() ?? string.Empty;
}