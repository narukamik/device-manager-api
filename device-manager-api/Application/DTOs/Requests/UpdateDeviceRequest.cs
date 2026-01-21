using device_manager_api.Domain.Enums;

namespace device_manager_api.Application.DTOs.Requests;

/// <summary>
/// Request to fully update an existing device
/// </summary>
public record UpdateDeviceRequest(
    string Name,
    string Brand,
    DeviceState State
);
