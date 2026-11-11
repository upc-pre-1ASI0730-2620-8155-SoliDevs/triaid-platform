using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Model.Aggregates;
using TriAidPlatform.API.SpecialtyAssignment.Domain.Repositories;
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
public class VouchersController(IVoucherRepository voucherRepository, IUnitOfWork unitOfWork) : ControllerBase
{
    /**
     * <summary>
     *     Lists every voucher across institutions; the web client
     *     filters by institution client-side (ADR-010)
     * </summary>
     * <param name="cancellationToken">Token to observe for cancellation.</param>
     * <returns>All vouchers.</returns>
     */
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
    public async Task<IActionResult> Create([FromBody] Voucher voucher, CancellationToken cancellationToken)
    {
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
}
