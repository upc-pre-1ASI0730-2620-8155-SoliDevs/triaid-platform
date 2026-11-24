namespace TriAidPlatform.API.PatientRegistration.Interfaces.REST.PatientPortal;

/// <summary>
/// Everything the patient portal shows, resolved for the signed-in patient only.
/// The client never passes a patient id: it comes from the token.
/// </summary>
/// <param name="Patient">Personal data (read only).</param>
/// <param name="Episode">Current or latest triage episode, null when the patient has none.</param>
/// <param name="Classification">Confirmed or suggested classification of the episode.</param>
/// <param name="Referral">Referral of the episode, null while not derived.</param>
/// <param name="Queue">Live queue position, null while not derived.</param>
/// <param name="Vitals">Latest vital signs reading of the episode, null when not captured yet.</param>
public record PatientOverviewResource(
    PatientPortalResource Patient,
    TriageEpisodeResource? Episode,
    ClassificationPortalResource? Classification,
    ReferralPortalResource? Referral,
    QueuePositionResource? Queue,
    VitalsPortalResource? Vitals);

/// <summary>
/// Vital signs captured by the staff, exactly as stored on the episode. Each entry
/// carries the reading value, its source (device or manual) and the capture time.
/// </summary>
/// <param name="BloodPressure">pa · arterial pressure, e.g. "142/88".</param>
/// <param name="OxygenSaturation">spo2 · percentage.</param>
/// <param name="HeartRate">fc · beats per minute.</param>
/// <param name="Temperature">temp · Celsius.</param>
public record VitalsPortalResource(VitalEntry? BloodPressure, VitalEntry? OxygenSaturation, VitalEntry? HeartRate, VitalEntry? Temperature);

/// <summary>One vital sign reading.</summary>
/// <param name="Value">Reading value as text.</param>
/// <param name="Source">device or manual.</param>
/// <param name="Time">Capture time as recorded, HH:mm.</param>
public record VitalEntry(string Value, string? Source, string? Time);

/// <summary>Patient data shown in the portal header and the "my data" card.</summary>
public record PatientPortalResource(int Id, string? Dni, string FullName, DateOnly BirthDate, string? Email);

/// <summary>Episode summary.</summary>
public record TriageEpisodeResource(int Id, DateTime ArrivalTime, string State);

/// <summary>Classification of the episode with the NT-158 level.</summary>
public record ClassificationPortalResource(string EpisodeId, int? Level, string State, DateTime? ConfirmedAt);

/// <summary>Referral: specialty, room and time.</summary>
public record ReferralPortalResource(int Id, string Specialty, string Room, DateTime ReferredAt, string State);

/// <summary>
/// Live queue position computed on every call, never stored.
/// </summary>
/// <param name="Ahead">Patients with the same priority waiting before this one.</param>
/// <param name="Total">Patients with the same priority waiting in the specialty.</param>
/// <param name="EstimatedWaitMinutes">Rough estimate, null when there is no basis.</param>
public record QueuePositionResource(int Ahead, int Total, int? EstimatedWaitMinutes);
