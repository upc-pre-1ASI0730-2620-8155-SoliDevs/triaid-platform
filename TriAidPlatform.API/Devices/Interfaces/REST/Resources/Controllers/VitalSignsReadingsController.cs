using TriAidPlatform.API.Shared.Infrastructure.Localization;
﻿using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using TriAidPlatform.API.Devices.Domain.Repositories;
using TriAidPlatform.API.Devices.Interfaces.REST.Resources;
using TriAidPlatform.API.Devices.Interfaces.REST.Transform;
using TriAidPlatform.API.Shared.Domain.Repositories;

namespace TriAidPlatform.API.Devices.Interfaces.REST.Controllers;

/// <summary>
/// REST Controller for managing Vital Signs Readings.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class VitalSignsReadingsController(
    IVitalSignsReadingRepository vitalSignsReadingRepository,
    IDeviceRepository deviceRepository,
    IUnitOfWork unitOfWork,
    IApiMessages messages) : ControllerBase
{
    /// <summary>
    /// Creates a new vital signs reading.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<VitalSignsReadingResource>> CreateVitalSignsReading([FromBody] CreateVitalSignsReadingResource resource, CancellationToken cancellationToken)
    {
        // Validate the device and get the id (EDeviceType)
        var device = await deviceRepository.FindByIdAsync(resource.DeviceId, cancellationToken);
        if (device is null) 
            return BadRequest(new { message = messages["DeviceNotFound"] });

        // Validate the vital signs values
        if (resource.HeartRate <= 0 || resource.HeartRate > 300)
            return BadRequest(new { message = messages["InvalidHeartRate"] });
            
        if (resource.SystolicPressure <= 0 || resource.DiastolicPressure <= 0 || resource.SystolicPressure <= resource.DiastolicPressure)
            return BadRequest(new { message = messages["InvalidBloodPressure"] });
            
        if (resource.OxygenSaturation < 0 || resource.OxygenSaturation > 100)
            return BadRequest(new { message = messages["InvalidOxygenSaturation"] });
            
        if (resource.Temperature < 30 || resource.Temperature > 45)
            return BadRequest(new { message = messages["InvalidTemperature"] });

        // creation and saving the vital signs reading
        var reading = VitalSignsReadingAssembler.ToModelFromCreate(resource, device.Type);
        
        await vitalSignsReadingRepository.AddAsync(reading, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        
        var readingResource = VitalSignsReadingAssembler.ToResourceFromEntity(reading);
        
        return CreatedAtAction(nameof(GetVitalSignsReadingById), new { id = readingResource.Id }, readingResource);
    }

    /// <summary>
    /// Gets a vital signs reading by its unique identifier.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<VitalSignsReadingResource>> GetVitalSignsReadingById(int id, CancellationToken cancellationToken)
    {
        var reading = await vitalSignsReadingRepository.FindByIdAsync(id, cancellationToken);
        if (reading is null) return NotFound();
        
        var readingResource = VitalSignsReadingAssembler.ToResourceFromEntity(reading);
        return Ok(readingResource);
    }

    /// <summary>
    /// Gets all registered vital signs readings.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<VitalSignsReadingResource>>> GetAllVitalSignsReadings(CancellationToken cancellationToken)
    {
        var readings = await vitalSignsReadingRepository.ListAsync(cancellationToken);
        var readingResources = readings.Select(VitalSignsReadingAssembler.ToResourceFromEntity);
        
        return Ok(readingResources);
    }
}