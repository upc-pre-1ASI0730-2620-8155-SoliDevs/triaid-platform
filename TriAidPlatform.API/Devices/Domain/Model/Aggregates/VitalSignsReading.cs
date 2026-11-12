using TriAidPlatform.API.Devices.Domain.Model.ValueObjects;

namespace TriAidPlatform.API.Devices.Domain.Model.Aggregates;

/// <summary>
/// Represents a biometric reading or clinical parameter captured by a medical device during a triage episode.
/// </summary>
public class VitalSignsReading
{
    public int Id { get; }
    public int EpisodeId { get; private set; }
    public int DeviceId { get; private set; }
    public EDeviceType DeviceType { get; private set; }
    public DateTime CapturedAt { get; private set; }
    
    // Vital signs parameters (nullable depending on device type)
    public int HeartRate { get; private set; }
    public int OxygenSaturation { get; private set; }
    public int SystolicBloodPressure { get; private set; }
    public int DiastolicBloodPressure { get; private set; }
    public decimal Temperature { get; private set; }
    public decimal? BloodGlucose { get; private set; }
 

    // Required by Entity Framework Core
    protected VitalSignsReading()
    {
        CapturedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="VitalSignsReading"/> class.
    /// </summary>
    public VitalSignsReading(
        int episodeId,
        int deviceId,
        EDeviceType deviceType,
        int heartRate,
        int oxygenSaturation,
        int systolicBloodPressure,
        int diastolicBloodPressure,
        decimal temperature,
        decimal? bloodGlucose = null)
    {
        if (episodeId <= 0)
            throw new ArgumentException("The episode ID must be a positive integer.", nameof(episodeId));
        if (deviceId <= 0)
            throw new ArgumentException("The device ID must be a positive integer.", nameof(deviceId));

        EpisodeId = episodeId;
        DeviceId = deviceId;
        DeviceType = deviceType;
        CapturedAt = DateTime.UtcNow;
        HeartRate = heartRate;
        OxygenSaturation = oxygenSaturation;
        SystolicBloodPressure = systolicBloodPressure;
        DiastolicBloodPressure = diastolicBloodPressure;
        Temperature = temperature;
        BloodGlucose = bloodGlucose;
    }
}