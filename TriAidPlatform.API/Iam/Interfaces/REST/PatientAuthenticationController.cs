using System.Net.Mime;
using TriAidPlatform.API.Iam.Application.Internal.CommandServices;
using TriAidPlatform.API.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using TriAidPlatform.API.Iam.Domain.Model.Aggregates;
using TriAidPlatform.API.Iam.Interfaces.Rest.Resources;
using TriAidPlatform.API.Iam.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;

namespace TriAidPlatform.API.Iam.Interfaces.REST;

/// <summary>
/// Authentication of the patient portal. Completely separate from
/// <see cref="AuthenticationController"/>: a patient token never opens the staff panel
/// and a staff token never opens the patient portal.
/// </summary>
/// <param name="patientSignInService">Patient sign-in use case.</param>
[ApiController]
[Route("auth/patient")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Patient Portal Authentication")]
public class PatientAuthenticationController(IPatientSignInService patientSignInService) : ControllerBase
{
    /// <summary>
    /// Signs a patient in with the DNI (or the e-mail given at admission) and the access code.
    /// </summary>
    /// <param name="resource">Sign-in credentials.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The patient session with its token.</returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [EndpointSummary("Patient portal sign-in")]
    [ProducesResponseType(typeof(AuthenticatedPatientResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SignIn([FromBody] PatientSignInResource resource, CancellationToken cancellationToken)
    {
        var command = PatientSignInCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await patientSignInService.Handle(command, cancellationToken);
        return result.Match<IActionResult>(
            onSuccess: r => Ok(new AuthenticatedPatientResource(
                r.Session.PatientId, r.Session.FullName, r.Session.Identifier,
                PatientSession.Role, r.Token)),
            onFailure: error => error.Type == "Forbidden"
                ? Forbid()
                : Unauthorized(new { error = error.Code }));
    }

    /// <summary>
    /// Returns the patient the current portal token belongs to.
    /// </summary>
    [HttpGet("me")]
    [AuthorizePatient]
    [EndpointSummary("Current patient session")]
    public IActionResult Me()
    {
        var patient = (PatientSession?)HttpContext.Items["Patient"];
        if (patient is null) return Unauthorized(new { error = "invalid-token" });
        return Ok(new { patientId = patient.PatientId, role = PatientSession.Role });
    }
}
