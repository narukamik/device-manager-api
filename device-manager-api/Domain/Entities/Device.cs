using device_manager_api.Domain.Enums;

namespace device_manager_api.Domain.Entities;

/// <summary>
/// Represents a device in the system
/// </summary>
public class Device
{
    /// <summary>
    /// Unique identifier for the device
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Name of the device
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Brand of the device
    /// </summary>
    public string Brand { get; set; } = string.Empty;
    
    /// <summary>
    /// Current operational state of the device
    /// </summary>
    public DeviceState State { get; set; }
    
    /// <summary>
    /// Timestamp when the device was created (immutable)
    /// </summary>
    public DateTime CreationTime { get; set; }
    
    /// <summary>
    /// Username of the user who created this device
    /// </summary>
    public string CreatedBy { get; set; } = string.Empty;
    
    /// <summary>
    /// Username of the user who last modified this device
    /// </summary>
    public string? ModifiedBy { get; set; }
    
    /// <summary>
    /// Timestamp when the device was last modified
    /// </summary>
    public DateTime? ModifiedAt { get; set; }
    
    /// <summary>
    /// Row version for optimistic concurrency control
    /// </summary>
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}
