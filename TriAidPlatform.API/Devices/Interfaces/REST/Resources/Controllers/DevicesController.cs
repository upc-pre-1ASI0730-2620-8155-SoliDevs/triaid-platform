using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using TriAidPlatform.API.Devices.Domain.Model.Aggregates;
using TriAidPlatform.API.Devices.Domain.Repositories;
using TriAidPlatform.API.Devices.Interfaces.REST.Resources;
using TriAidPlatform.API.Devices.Interfaces.REST.Transform;
using TriAidPlatform.API.Shared.Domain.Repositories;

namespace TriAidPlatform.API.Devices.Interfaces.REST.Controllers;

/// <summary>
/// REST controller for managing devices.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class DevicesController(
    IDeviceRepository deviceRepository,
    IUnitOfWork unitOfWork) : ControllerBase
{
    /// <summary>
    /// Creates a new device.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<DeviceResource>> CreateDevice(
        [FromBody] CreateDeviceResource resource,
        CancellationToken cancellationToken)
    {
        try
        {
            var command = CreateDeviceCommandFromResourceAssembler.ToCommandFromResource(resource);
            var device = new Device(command);
            await deviceRepository.AddAsync(device, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);

            var deviceResource = DeviceResourceFromEntityAssembler.ToResourceFromEntity(device);
            return CreatedAtAction(nameof(GetDeviceById), new { id = deviceResource.Id }, deviceResource);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Gets a device by its unique identifier.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<DeviceResource>> GetDeviceById(int id, CancellationToken cancellationToken)
    {
        var device = await deviceRepository.FindByIdAsync(id, cancellationToken);
        if (device is null) return NotFound();

        return Ok(DeviceResourceFromEntityAssembler.ToResourceFromEntity(device));
    }

    /// <summary>
    /// Gets all registered devices.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeviceResource>>> GetAllDevices(CancellationToken cancellationToken)
    {
        var devices = await deviceRepository.ListAsync(cancellationToken);
        var deviceResources = devices.Select(DeviceResourceFromEntityAssembler.ToResourceFromEntity);

        return Ok(deviceResources);
    }
}