namespace TriAidPlatform.API.PatientRegistration.Domain.Model.Events;

/// <summary>
/// Domain event raised when a new patient is registered in the platform,
/// either with an identity document or as an emergency temporary profile.
/// </summary>
/// <param name="PatientId">Identifier of the registered patient.</param>
/// <param name="OccurredAt">UTC date and time when the registration happened.</param>
public record PatientRegisteredEvent(int PatientId, DateTime OccurredAt);
