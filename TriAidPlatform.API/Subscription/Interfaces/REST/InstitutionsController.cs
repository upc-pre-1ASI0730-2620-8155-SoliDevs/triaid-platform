using Microsoft.AspNetCore.Mvc;
using TriAidPlatform.API.Subscription.Domain.Model;
using TriAidPlatform.API.Subscription.Domain.Repositories;
using TriAidPlatform.API.Subscription.Interfaces.REST.Resources;
using TriAidPlatform.API.Subscription.Interfaces.REST.Transform;

namespace TriAidPlatform.API.Subscription.Interfaces.REST;

/**
 * REST facade for the Institution aggregate (US54). Exposes the
 * contracting party of every subscription under /institutions.
 */
[ApiController]
[Route("institutions")]
public class InstitutionsController(IInstitutionRepository repository) : ControllerBase
{
    /// <summary>Lists every registered institution.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<InstitutionResource>>> GetAll(CancellationToken cancellationToken)
    {
        var institutions = await repository.ListAsync(cancellationToken);
        return Ok(institutions.Select(InstitutionResourceFromEntityAssembler.ToResource));
    }

    /// <summary>Gets a single institution by its identifier.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<InstitutionResource>> GetById(int id, CancellationToken cancellationToken)
    {
        var institution = await repository.FindByIdAsync(id, cancellationToken);
        return institution is null
            ? NotFound(new { message = $"Institution with id {id} was not found." })
            : Ok(InstitutionResourceFromEntityAssembler.ToResource(institution));
    }

    /// <summary>Registers a new contracting institution (US54).</summary>
    [HttpPost]
    public async Task<ActionResult<InstitutionResource>> Create(
        [FromBody] CreateInstitutionResource resource, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(resource.Name))
            return BadRequest(new { message = "The institution name is required." });

        if (await repository.ExistsByRucAsync(resource.Ruc, cancellationToken))
            return Conflict(new { message = $"An institution with RUC {resource.Ruc} already exists." });

        if (await repository.FindByNameAsync(resource.Name.Trim(), cancellationToken) is not null)
            return Conflict(new { message = $"An institution named '{resource.Name}' already exists." });

        var institution = new Institution
        {
            Name = resource.Name.Trim(),
            Ruc = resource.Ruc,
            Address = resource.Address,
            Phone = resource.Phone,
            Email = resource.Email,
            Active = true
        };

        await repository.AddAsync(institution, cancellationToken);
        await repository.CompleteAsync(cancellationToken);

        var created = InstitutionResourceFromEntityAssembler.ToResource(institution);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}
