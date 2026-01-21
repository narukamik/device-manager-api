using device_manager_api.Domain.Enums;

namespace device_manager_api.Application.DTOs.Requests;

/// <summary>
/// DTO for partial device updates via JSON Patch (only allows patching specific fields)
/// </summary>
public class DevicePatchDto
{
    /// <summary>
    /// Name of the device
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Brand of the device
    /// </summary>
    public string? Brand { get; set; }
    
    /// <summary>
    /// Current state of the device
    /// </summary>
    public DeviceState? State { get; set; }
}
