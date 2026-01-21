using device_manager_api.Application.DTOs.Requests;
using device_manager_api.Application.DTOs.Responses;
using device_manager_api.Domain.Entities;
using device_manager_api.Domain.Enums;
using Microsoft.AspNetCore.JsonPatch;

namespace device_manager_api.Application.Interfaces;

/// <summary>
/// Service interface for device business logic
/// </summary>
public interface IDeviceService
{
    Task<DeviceResponse?> GetByIdAsync(Guid id);
    Task<PaginatedResponse<DeviceResponse>> GetAllAsync(int pageNumber, int pageSize);
    Task<PaginatedResponse<DeviceResponse>> GetByBrandAsync(string brand, int pageNumber, int pageSize);
    Task<PaginatedResponse<DeviceResponse>> GetByStateAsync(DeviceState state, int pageNumber, int pageSize);
    Task<DeviceResponse> CreateAsync(CreateDeviceRequest request, string createdBy);
    Task<DeviceResponse> UpdateAsync(Guid id, UpdateDeviceRequest request);
    Task<DeviceResponse> PatchAsync(Guid id, JsonPatchDocument<DevicePatchDto> patchDoc);
    Task DeleteAsync(Guid id);
}
