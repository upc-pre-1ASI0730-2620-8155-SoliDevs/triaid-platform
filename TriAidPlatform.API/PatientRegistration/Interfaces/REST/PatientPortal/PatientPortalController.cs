using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using TriAidPlatform.API.Iam.Domain.Model.Aggregates;
using TriAidPlatform.API.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using TriAidPlatform.API.PatientRegistration.Domain.Model.Aggregates;
using TriAidPlatform.API.PatientRegistration.Domain.Repositories;
using TriAidPlatform.API.PatientRegistration.Interfaces.REST.PatientPortal;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Aggregates;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Repositories;
using TriAidPlatform.API.TriageClassification.Domain.Model.Aggregates;
using TriAidPlatform.API.TriageClassification.Domain.Model.ValueObjects;
using TriAidPlatform.API.TriageClassification.Domain.Repositories;

namespace TriAidPlatform.API.PatientRegistration.Interfaces.REST.PatientPortal;

/// <summary>
/// Read-only endpoints of the patient portal. Every response is scoped to the
/// patient identified by the bearer token: the client never sends a patient id,
/// so one patient can never read another one's record.
/// </summary>
/// <param name="patientRepository">Patient lookup.</param>
/// <param name="episodeRepository">Episode lookup.</param>
/// <param name="classificationRepository">Classification lookup.</param>
/// <param name="referralRepository">Referral lookup.</param>
/// <param name="specialtyRepository">Specialty catalog.</param>
[ApiController]
[Route("portal")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Patient Portal")]
[AuthorizePatient]
public class PatientPortalController(
    IPatientRepository patientRepository,
    ITriageEpisodeRepository episodeRepository,
    IClassificationRepository classificationRepository,
    IReferralRepository referralRepository,
    ISpecialtyRepository specialtyRepository) : ControllerBase
{
    /// <summary>Average minutes per patient used to estimate the wait when there is no history.</summary>
    private const int MinutesPerPatient = 8;

    private PatientSession Current =>
        (PatientSession?)HttpContext.Items["Patient"] ?? throw new UnauthorizedAccessException();

    /// <summary>
    /// Everything the portal needs in one call: patient, latest episode, classification,
    /// referral and the live queue position.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet("overview")]
    [EndpointSummary("Patient overview")]
    [ProducesResponseType(typeof(PatientOverviewResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PatientOverviewResource>> Overview(CancellationToken cancellationToken)
    {
        var session = Current;
        var patient = await patientRepository.FindByIdAsync(session.PatientId, cancellationToken);
        if (patient is null) return NotFound(new { message = "Patient not found." });

        var episode = (await episodeRepository.FindByPatientIdAsync(session.PatientId, cancellationToken))
            .OrderByDescending(e => e.ArrivalTime)
            .FirstOrDefault();

        Classification? classification = null;
        Referral? referral = null;
        QueuePositionResource? queue = null;
        string? specialtyName = null;

        if (episode is not null)
        {
            var episodeId = episode.Id.ToString();
            classification = await classificationRepository.FindByEpisodeIdAsync(episodeId, cancellationToken);
            referral = await referralRepository.FindByEpisodeIdAsync(episodeId, cancellationToken);

            if (referral is not null)
            {
                var specialty = await specialtyRepository.FindByIdAsync(referral.SpecialtyId, cancellationToken);
                specialtyName = specialty?.Name ?? $"Specialty {referral.SpecialtyId}";
                queue = await ComputeQueueAsync(episodeId, referral, classification, cancellationToken);
            }
        }

        var fullName = $"{patient.FirstName} {patient.LastName}".Trim();
        return Ok(new PatientOverviewResource(
            new PatientPortalResource(patient.Id, patient.Dni, fullName, patient.BirthDate, patient.Email),
            episode is null ? null : new TriageEpisodeResource(episode.Id, episode.ArrivalTime, episode.State.ToString()),
            classification is null ? null : new ClassificationPortalResource(
                episode!.Id.ToString(),
                classification.State == EClassificationState.Confirmed ? (int)classification.FinalLevel : (int)classification.SuggestedLevel,
                classification.State.ToString(),
                classification.ConfirmedAt),
            referral is null ? null : new ReferralPortalResource(
                referral.Id, specialtyName ?? string.Empty, referral.Room, referral.ReferredAt, referral.State),
            queue));
    }

    /// <summary>
    /// Live queue position for the patient's current referral. Polled every 45 seconds
    /// while the "my turn" screen is visible.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet("queue")]
    [EndpointSummary("Live queue position")]
    [ProducesResponseType(typeof(QueuePositionResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QueuePositionResource>> Queue(CancellationToken cancellationToken)
    {
        var session = Current;
        var episode = (await episodeRepository.FindByPatientIdAsync(session.PatientId, cancellationToken))
            .OrderByDescending(e => e.ArrivalTime)
            .FirstOrDefault();
        if (episode is null) return NotFound(new { message = "No episode found." });

        var episodeId = episode.Id.ToString();
        var referral = await referralRepository.FindByEpisodeIdAsync(episodeId, cancellationToken);
        if (referral is null) return NotFound(new { message = "Not referred yet." });

        var classification = await classificationRepository.FindByEpisodeIdAsync(episodeId, cancellationToken);
        return Ok(await ComputeQueueAsync(episodeId, referral, classification, cancellationToken));
    }

    /// <summary>
    /// Computes how many patients with the same priority are waiting before this one
    /// in the same specialty. Counted on every call from the live referrals, never stored,
    /// so the number always reflects the real queue.
    /// </summary>
    private async Task<QueuePositionResource> ComputeQueueAsync(
        string episodeId, Referral referral, Classification? classification, CancellationToken cancellationToken)
    {
        if (referral.State != "waiting") return new QueuePositionResource(0, 0, 0);

        var level = classification is null ? (int?)null : (int)classification.FinalLevel;
        var waiting = (await referralRepository.ListAsync(cancellationToken))
            .Where(r => r.SpecialtyId == referral.SpecialtyId && r.State == "waiting")
            .ToList();

        // Priority of each waiting referral, resolved through its classification.
        var priorities = new Dictionary<int, int?>();
        foreach (var r in waiting)
            priorities[r.Id] = (await classificationRepository.FindByEpisodeIdAsync(r.EpisodeId, cancellationToken)) is { } c
                ? (int)c.FinalLevel : (int?)null;

        var samePriority = waiting.Where(r => priorities[r.Id] == level).ToList();
        var ahead = samePriority.Count(r => r.ReferredAt < referral.ReferredAt);
        var total = samePriority.Count;

        return new QueuePositionResource(ahead, total, ahead * MinutesPerPatient);
    }
}
