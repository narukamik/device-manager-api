using device_manager_api.Application.DTOs.Requests;
using device_manager_api.Application.DTOs.Responses;

namespace device_manager_api.Application.Interfaces;

/// <summary>
/// Service interface for authentication operations
/// </summary>
public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<LoginResponse?> RefreshTokenAsync(RefreshTokenRequest request);
    Task<bool> ChangePasswordAsync(string username, ChangePasswordRequest request);
}
