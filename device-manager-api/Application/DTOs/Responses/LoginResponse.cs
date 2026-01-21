namespace device_manager_api.Application.DTOs.Responses;

/// <summary>
/// Response containing authentication tokens
/// </summary>
public record LoginResponse(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn
);
