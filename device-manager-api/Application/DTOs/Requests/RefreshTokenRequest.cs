namespace device_manager_api.Application.DTOs.Requests;

/// <summary>
/// Request to refresh an access token
/// </summary>
public record RefreshTokenRequest(
    string RefreshToken
);
