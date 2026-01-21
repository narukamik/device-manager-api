using device_manager_api.Domain.Enums;

namespace device_manager_api.Application.DTOs.Requests;

/// <summary>
/// Request to create a new device
/// </summary>
public record CreateDeviceRequest(
    string Name,
    string Brand,
    DeviceState State
);
