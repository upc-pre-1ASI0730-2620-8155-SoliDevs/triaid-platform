namespace TriAidPlatform.API.SpecialtyAssignment.Interfaces.REST.Resources;

/// <summary>
/// Patient behind a voucher code scanned at the admission desk. Enough for the
/// staff to confirm the identity and open the patient file.
/// </summary>
/// <param name="EpisodeId">Episode identifier.</param>
/// <param name="PatientId">Patient identifier.</param>
/// <param name="EpisodeState">Episode state.</param>
/// <param name="PatientName">Full name.</param>
/// <param name="Dni">Identity document, null for temporary profiles.</param>
/// <param name="ArrivalTime">When the patient arrived.</param>
/// <param name="SpecialtyId">Specialty the patient was referred to, null while not referred.</param>
/// <param name="Room">Consulting room, null while not referred.</param>
/// <param name="ReferralState">waiting / attended.</param>
public record ScannedVoucherResource(
    int EpisodeId, int PatientId, string EpisodeState, string PatientName, string? Dni, DateTime ArrivalTime,
    int? SpecialtyId, string? Room, string? ReferralState);
