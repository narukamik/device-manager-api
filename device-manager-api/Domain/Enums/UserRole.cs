namespace device_manager_api.Domain.Enums;

/// <summary>
/// Represents the role of a user in the system
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Full access to all resources and operations
    /// </summary>
    Admin = 0,
    
    /// <summary>
    /// Can create, read, and update devices but cannot delete
    /// </summary>
    Manager = 1,
    
    /// <summary>
    /// Read-only access to devices
    /// </summary>
    Viewer = 2
}
