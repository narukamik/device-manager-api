namespace device_manager_api.Application.DTOs.Requests;

/// <summary>
/// Request to change user password
/// </summary>
public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword
);
