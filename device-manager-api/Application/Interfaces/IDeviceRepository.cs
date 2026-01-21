using device_manager_api.Domain.Entities;
using device_manager_api.Domain.Enums;
using Microsoft.AspNetCore.JsonPatch;

namespace device_manager_api.Application.Interfaces;

/// <summary>
/// Repository interface for Device operations
/// </summary>
public interface IDeviceRepository
{
    Task<Device?> GetByIdAsync(Guid id);
    Task<(IEnumerable<Device> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize);
    Task<(IEnumerable<Device> Items, int TotalCount)> GetByBrandAsync(string brand, int pageNumber, int pageSize);
    Task<(IEnumerable<Device> Items, int TotalCount)> GetByStateAsync(DeviceState state, int pageNumber, int pageSize);
    Task<Device> CreateAsync(Device device);
    Task<Device> UpdateAsync(Device device);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
