namespace TriAidPlatform.API.Iam.Domain.Model.Commands;

/// <summary>
/// Signs a patient in to the patient portal with the DNI (or the e-mail given
/// at admission) and the access code handed over by the admission desk.
/// </summary>
/// <param name="Identifier">DNI or e-mail.</param>
/// <param name="Password">Plain access code.</param>
public record PatientSignInCommand(string Identifier, string Password);
