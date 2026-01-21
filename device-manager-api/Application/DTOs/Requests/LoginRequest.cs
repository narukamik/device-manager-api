namespace device_manager_api.Application.DTOs.Requests;

/// <summary>
/// Request to authenticate a user
/// </summary>
public record LoginRequest(
    string Username,
    string Password
);
