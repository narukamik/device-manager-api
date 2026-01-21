using Asp.Versioning;
using device_manager_api.Application.DTOs.Requests;
using device_manager_api.Application.DTOs.Responses;
using device_manager_api.Application.Interfaces;
using device_manager_api.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace device_manager_api.API.Controllers.v1;

/// <summary>
/// Devices management controller
/// </summary>
[ApiController]
[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/devices")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceService _deviceService;

    public DevicesController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    /// <summary>
    /// Get devices with optional filtering and pagination
    /// </summary>
    /// <param name="id">Optional device ID to get single device</param>
    /// <param name="brand">Optional brand filter</param>
    /// <param name="state">Optional state filter</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <returns>Device(s) matching the criteria</returns>
    [HttpGet]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(PaginatedResponse<DeviceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDevices(
        [FromQuery] Guid? id,
        [FromQuery] string? brand,
        [FromQuery] DeviceState? state,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        // Single device by ID
        if (id.HasValue)
        {
            var device = await _deviceService.GetByIdAsync(id.Value);
            if (device == null)
                return NotFound(new { message = $"Device with ID {id} not found" });
            
            return Ok(device);
        }

        // Filter by brand
        if (!string.IsNullOrEmpty(brand))
        {
            var devices = await _deviceService.GetByBrandAsync(brand, pageNumber, pageSize);
            return Ok(devices);
        }

        // Filter by state
        if (state.HasValue)
        {
            var devices = await _deviceService.GetByStateAsync(state.Value, pageNumber, pageSize);
            return Ok(devices);
        }

        // Get all devices
        var allDevices = await _deviceService.GetAllAsync(pageNumber, pageSize);
        return Ok(allDevices);
    }

    /// <summary>
    /// Create a new device
    /// </summary>
    /// <param name="request">Device creation request</param>
    /// <returns>Created device</returns>
    [HttpPost]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateDevice([FromBody] CreateDeviceRequest request)
    {
        var username = User.Identity?.Name ?? "Unknown";
        var device = await _deviceService.CreateAsync(request, username);
        
        return CreatedAtAction(
            nameof(GetDevices), 
            new { id = device.Id }, 
            device);
    }

    /// <summary>
    /// Fully update an existing device
    /// </summary>
    /// <param name="id">Device ID</param>
    /// <param name="request">Device update request</param>
    /// <returns>Updated device</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateDevice(Guid id, [FromBody] UpdateDeviceRequest request)
    {
        var device = await _deviceService.UpdateAsync(id, request);
        return Ok(device);
    }

    /// <summary>
    /// Partially update an existing device using JSON Patch (RFC 6902)
    /// </summary>
    /// <param name="id">Device ID</param>
    /// <param name="patchDoc">JSON Patch document</param>
    /// <returns>Updated device</returns>
    [HttpPatch("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(typeof(DeviceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> PatchDevice(Guid id, [FromBody] JsonPatchDocument<DevicePatchDto> patchDoc)
    {
        if (patchDoc == null)
            return BadRequest(new { message = "Patch document is required" });

        var device = await _deviceService.PatchAsync(id, patchDoc);
        return Ok(device);
    }

    /// <summary>
    /// Delete a device
    /// </summary>
    /// <param name="id">Device ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteDevice(Guid id)
    {
        await _deviceService.DeleteAsync(id);
        return NoContent();
    }
}
