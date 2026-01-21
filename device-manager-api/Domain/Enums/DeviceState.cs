namespace device_manager_api.Domain.Enums;

/// <summary>
/// Represents the operational state of a device
/// </summary>
public enum DeviceState
{
    /// <summary>
    /// Device is available for use
    /// </summary>
    Available = 0,
    
    /// <summary>
    /// Device is currently in use
    /// </summary>
    InUse = 1,
    
    /// <summary>
    /// Device is inactive
    /// </summary>
    Inactive = 2
}
