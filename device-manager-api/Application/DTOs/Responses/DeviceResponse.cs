using device_manager_api.Domain.Enums;

namespace device_manager_api.Application.DTOs.Responses;

/// <summary>
/// Response containing device information
/// </summary>
public record DeviceResponse(
    Guid Id,
    string Name,
    string Brand,
    DeviceState State,
    DateTime CreationTime,
    string CreatedBy,
    string? ModifiedBy,
    DateTime? ModifiedAt
);
