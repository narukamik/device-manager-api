using device_manager_api.Domain.Entities;

namespace device_manager_api.Application.Interfaces;

/// <summary>
/// Service interface for JWT token operations
/// </summary>
public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    int GetAccessTokenExpirationMinutes();
}
