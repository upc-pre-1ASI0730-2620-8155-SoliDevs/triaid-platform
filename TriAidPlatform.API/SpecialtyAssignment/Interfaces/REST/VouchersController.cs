using TriAidPlatform.API.Shared.Infrastructure.Localization;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Aggregates;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Repositories;
using TriAidPlatform.API.SpecialtyAssignment.Interfaces.REST.Resources;
using TriAidPlatform.API.Iam.Infrastructure.Tokens.Jwt.Configuration;
using TriAidPlatform.API.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Microsoft.Extensions.Options;
using TriAidPlatform.API.PatientRegistration.Domain.Repositories;
using System.Text.RegularExpressions;
using TriAidPlatform.API.Shared.Infrastructure.Security;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Aggregates;
using TriAidPlatform.API.Shared.Domain.Repositories;

namespace TriAidPlatform.API.SpecialtyAssignment.Interfaces.REST;

/**
 * <summary>
 *     Voucher REST facade. Vouchers are receipts of a referral sent to
 *     the patient portal or front desk; the web client keeps full CRUD
 *     ownership of the record (ADR-010 backend swap contract).
 * </summary>
 * <param name="voucherRepository">Repository of the Voucher entity.</param>
 * <param name="unitOfWork">Unit of work used to commit the changes.</param>
 */
[ApiController]
[Route("vouchers")]
[Produces(MediaTypeNames.Application.Json)]
[Tags("Vouchers")]
// Voucher codes resolve into patient identity: staff session required.
[Authorize]
public class VouchersController(
    IVoucherRepository voucherRepository,
    IReferralRepository referralRepository,
    IPatientRepository patientRepository,
    ITriageEpisodeRepository episodeRepository,
    ISpecialtyRepository specialtyRepository,
    VoucherCodeCipher cipher,
    IUnitOfWork unitOfWork,
    Shared.Infrastructure.Email.IEmailSender emailSender,
    IApiMessages messages) : ControllerBase
{
    /**
     * <summary>
     *     Lists every voucher across institutions; the web client
     *     filters by institution client-side (ADR-010)
     * </summary>
     * <param name="cancellationToken">Token to observe for cancellation.</param>
     * <returns>All vouchers.</returns>
     */
    /// <summary>
    /// Issues the voucher of a referral: builds the payload from the stored data,
    /// seals it into an opaque token (nothing readable leaves the API) and
    /// persists it as the voucher QR. Returns the token to render as QR.
    /// </summary>
    /// <param name="referralId">Referral identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost("referral/{referralId:int}/issue")]
    [EndpointSummary("Issue the sealed voucher code of a referral")]
    [ProducesResponseType(typeof(IssuedVoucherResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Issue(int referralId, CancellationToken cancellationToken)
    {
        var referral = await referralRepository.FindByIdAsync(referralId, cancellationToken);
        if (referral is null) return NotFound(new { message = messages["VoucherReferralNotFound", referralId] });

        var plain = $"TRI-AID|{referral.EpisodeId}|{referral.Room}|{referral.QueuePosition}";
        var token = cipher.Seal(plain);

        var existing = (await voucherRepository.ListAsync(cancellationToken))
            .FirstOrDefault(v => v.ReferralId == referralId);
        if (existing is null)
            await voucherRepository.AddAsync(new Voucher { ReferralId = referralId, QrCode = token, Channel = "QrPortal", Institution = referral.Institution }, cancellationToken);
        else { existing.QrCode = token; voucherRepository.Update(existing); }
        await unitOfWork.CompleteAsync(cancellationToken);

        return Ok(new IssuedVoucherResource(referralId, referral.EpisodeId, token));
    }

    /// <summary>
    /// Resolves a voucher or episode code scanned at the admission desk into the
    /// patient it belongs to, so the staff jumps straight to the patient file.
    /// Accepts the structured QR payload ("TRI-AID|episode|specialty|room|#pos")
    /// or the plain episode code ("EP-0014" / "14").
    /// </summary>
    /// <param name="code">Raw QR payload or episode code.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet("verify/{code}")]
    [EndpointSummary("Resolve a scanned voucher code into its patient")]
    [ProducesResponseType(typeof(ScannedVoucherResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Verify(string code, CancellationToken cancellationToken)
    {
        var episodeId = ParseEpisodeId(code);
        if (episodeId is null) return NotFound(new { message = messages["CodeNotEpisode"] });

        var episode = await episodeRepository.FindByIdAsync(episodeId.Value, cancellationToken);
        if (episode is null) return NotFound(new { message = messages["VoucherEpisodeNotFound", episodeId] });

        var patient = await patientRepository.FindByIdAsync(episode.PatientId, cancellationToken);
        var referral = await referralRepository.FindByEpisodeIdAsync(episode.Id.ToString(), cancellationToken);

        return Ok(new ScannedVoucherResource(
            episode.Id, episode.PatientId, episode.State.ToString(),
            patient is null ? string.Empty : $"{patient.FirstName} {patient.LastName}".Trim(),
            patient?.Dni, episode.ArrivalTime,
            referral?.SpecialtyId, referral is null ? null : referral.Room, referral?.State));
    }

    /// <summary>Extracts the numeric episode id from a QR payload or a plain code.</summary>
    private int? ParseEpisodeId(string code)
    {
        if (string.IsNullOrWhiteSpace(code)) return null;
        var raw = Uri.UnescapeDataString(code).Trim();

        // Sealed token first: the only form the API issues from now on.
        var opened = cipher.Unseal(raw);
        if (opened is not null) raw = opened;
        if (raw.StartsWith("TRI-AID|", StringComparison.OrdinalIgnoreCase))
            raw = raw.Split('|')[1];
        raw = Regex.Replace(raw, @"^EP-0*", string.Empty, RegexOptions.IgnoreCase);
        return int.TryParse(raw, out var id) ? id : null;
    }

    [HttpGet]
    [EndpointSummary("List all vouchers")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var vouchers = await voucherRepository.ListAsync(cancellationToken);
        return Ok(vouchers);
    }

    /**
     * <summary>
     *     Creates a voucher from the web client record
     * </summary>
     * <param name="voucher">The voucher record sent by the client.</param>
     * <param name="cancellationToken">Token to observe for cancellation.</param>
     * <returns>The created voucher with its identifier.</returns>
     * <response code="200">Voucher created.</response>
     */
    [HttpPost]
    [EndpointSummary("Create a voucher")]
    public async Task<IActionResult> Create([FromBody] SaveVoucherResource resource, CancellationToken cancellationToken)
    {
        var voucher = new Voucher
        {
            ReferralId = resource.ReferralId,
            QrCode = resource.QrCode,
            Channel = resource.Channel,
            SentAt = resource.SentAt,
            Institution = resource.Institution
        };
        await voucherRepository.AddAsync(voucher, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return Ok(voucher);
    }

    /**
     * <summary>
     *     Applies a full-record voucher update from the web client
     * </summary>
     * <param name="id">Identifier of the voucher.</param>
     * <param name="voucher">The voucher record sent by the client.</param>
     * <param name="cancellationToken">Token to observe for cancellation.</param>
     * <returns>The updated voucher.</returns>
     * <response code="200">Voucher updated.</response>
     * <response code="404">Voucher not found.</response>
     */
    [HttpPut("{id:int}")]
    [EndpointSummary("Update a voucher")]
    public async Task<IActionResult> Update(int id, [FromBody] Voucher voucher, CancellationToken cancellationToken)
    {
        var target = await voucherRepository.FindByIdAsync(id, cancellationToken);
        if (target is null)
            return NotFound(new { error = "voucher-not-found" });

        target.ReferralId = voucher.ReferralId;
        target.QrCode = voucher.QrCode;
        target.Channel = voucher.Channel;
        target.SentAt = voucher.SentAt;
        target.Institution = voucher.Institution;
        await unitOfWork.CompleteAsync(cancellationToken);
        return Ok(target);
    }

    /// <summary>E-mails the voucher of a referral; optionally stores the address as the patient's e-mail.</summary>
    /// <param name="referralId">Identifier of the referral.</param>
    /// <param name="resource">Recipient address and whether to attach it to the patient.</param>
    /// <param name="cancellationToken">Token to observe for cancellation.</param>
    [HttpPost("referral/{referralId:int}/email")]
    [EndpointSummary("E-mail the voucher of a referral")]
    [ProducesResponseType(typeof(EmailVoucherSentResource), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> Email(int referralId, [FromBody] EmailVoucherResource resource, CancellationToken cancellationToken)
    {
        var email = resource.Email?.Trim().ToLowerInvariant() ?? string.Empty;
        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            return BadRequest(new { message = messages["EmailRequired"] });
        if (!emailSender.IsConfigured)
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new { message = messages["EmailNotConfigured"] });

        var referral = await referralRepository.FindByIdAsync(referralId, cancellationToken);
        if (referral is null) return NotFound(new { message = messages["VoucherReferralNotFound", referralId] });
        var episode = await episodeRepository.FindByIdAsync(int.Parse(referral.EpisodeId), cancellationToken);
        var patient = episode is null ? null : await patientRepository.FindByIdAsync(episode.PatientId, cancellationToken);
        var specialty = (await specialtyRepository.ListAsync(cancellationToken)).FirstOrDefault(s => s.Id == referral.SpecialtyId);

        if (resource.AttachToPatient && patient is not null)
        {
            patient.UpdateEmail(email);
            patientRepository.Update(patient);
            await unitOfWork.CompleteAsync(cancellationToken);
        }

        var token = cipher.Seal($"TRI-AID|{referral.EpisodeId}|{referral.Room}|{referral.QueuePosition}");
        using var qr = new QRCodeGenerator().CreateQrCode(token, QRCodeGenerator.ECCLevel.Q);
        var qrImage = Convert.ToBase64String(new PngByteQRCode(qr).GetGraphic(12));
        var name = patient is null ? string.Empty : $"{patient.FirstName} {patient.LastName}".Trim();
        var html = VoucherEmailHtml(name, specialty?.Name ?? "Especialidad", referral.Room, referral.QueuePosition, referral.EpisodeId, qrImage);

        var sent = await emailSender.SendAsync(new Shared.Infrastructure.Email.EmailMessage(email, $"Tu comprobante de derivación · Tri-Aid · EP-{referral.EpisodeId}", html), cancellationToken);
        if (!sent) return StatusCode(StatusCodes.Status502BadGateway, new { message = messages["EmailRejected"] });

        return Ok(new EmailVoucherSentResource(email, resource.AttachToPatient && patient is not null));
    }

    private static string VoucherEmailHtml(string patientName, string specialty, string room, int position, string episodeId, string qrPngBase64) => """
                <div style="font-family:Inter,Arial,sans-serif;max-width:520px;margin:0 auto;border:1px solid #e3e8e5;border-radius:18px;overflow:hidden">
          <div style="background:#0a2118;color:#fff;padding:18px 24px;font-weight:700">Tri-Aid · Comprobante de derivación</div>
          <div style="padding:24px">
            <p style="margin:0 0 14px;color:#15291f;font-size:15px">Hola {{name}}, tu derivación quedó registrada:</p>
            <table style="width:100%;border-collapse:collapse;font-size:14px;color:#15291f">
              <tr><td style="padding:6px 0;color:#5f6b66">Especialidad</td><td style="padding:6px 0;text-align:right;font-weight:600">{{spec}}</td></tr>
              <tr><td style="padding:6px 0;color:#5f6b66">Consultorio</td><td style="padding:6px 0;text-align:right;font-weight:600">{{room}}</td></tr>
              <tr><td style="padding:6px 0;color:#5f6b66">Posición en cola</td><td style="padding:6px 0;text-align:right;font-weight:600">#{{pos}}</td></tr>
              <tr><td style="padding:6px 0;color:#5f6b66">Episodio</td><td style="padding:6px 0;text-align:right;font-weight:600">EP-{{ep}}</td></tr>
            </table>
            <div style="margin:18px 0 8px;text-align:center"><img src="data:image/png;base64,{{qr}}" width="220" height="220" alt="QR del comprobante" style="border-radius:12px"/></div>
            <p style="margin:0;color:#5f6b66;font-size:13px;text-align:center">Muestra este QR al ingresar al consultorio. También puedes verlo en el portal del paciente: <a href="https://triaid-solidevs.web.app/portal" style="color:#0f6b45">triaid-solidevs.web.app/portal</a></p>
          </div>
        </div>
        """
        .Replace("{{name}}", System.Net.WebUtility.HtmlEncode(patientName))
        .Replace("{{spec}}", System.Net.WebUtility.HtmlEncode(specialty))
        .Replace("{{room}}", System.Net.WebUtility.HtmlEncode(room))
        .Replace("{{pos}}", position.ToString())
        .Replace("{{ep}}", System.Net.WebUtility.HtmlEncode(episodeId.PadLeft(4, '0')))
        .Replace("{{qr}}", qrPngBase64);

}
