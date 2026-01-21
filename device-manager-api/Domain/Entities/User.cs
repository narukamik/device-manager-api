using device_manager_api.Domain.Enums;

namespace device_manager_api.Domain.Entities;

/// <summary>
/// Represents a user in the system
/// </summary>
public class User
{
    /// <summary>
    /// Unique identifier for the user
    /// </summary>
    public Guid Id { get; set; }
    
    /// <summary>
    /// Username for authentication
    /// </summary>
    public string Username { get; set; } = string.Empty;
    
    /// <summary>
    /// BCrypt hashed password
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;
    
    /// <summary>
    /// Role of the user in the system
    /// </summary>
    public UserRole Role { get; set; }
    
    /// <summary>
    /// Current refresh token for JWT authentication
    /// </summary>
    public string? RefreshToken { get; set; }
    
    /// <summary>
    /// Expiry time for the refresh token
    /// </summary>
    public DateTime? RefreshTokenExpiryTime { get; set; }
}
