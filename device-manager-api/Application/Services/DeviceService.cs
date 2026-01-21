using AutoMapper;
using device_manager_api.Application.DTOs.Requests;
using device_manager_api.Application.DTOs.Responses;
using device_manager_api.Application.Exceptions;
using device_manager_api.Application.Interfaces;
using device_manager_api.Domain.Entities;
using device_manager_api.Domain.Enums;
using Microsoft.AspNetCore.JsonPatch;

namespace device_manager_api.Application.Services;

/// <summary>
/// Service implementing device business logic
/// </summary>
public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _repository;
    private readonly ICacheService _cache;
    private readonly IMapper _mapper;

    // Cache TTL constants
    private static readonly TimeSpan SingleDeviceTtl = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan CollectionTtl = TimeSpan.FromMinutes(2);
    private static readonly TimeSpan AllDevicesTtl = TimeSpan.FromMinutes(1);

    public DeviceService(IDeviceRepository repository, ICacheService cache, IMapper mapper)
    {
        _repository = repository;
        _cache = cache;
        _mapper = mapper;
    }

    public async Task<DeviceResponse?> GetByIdAsync(Guid id)
    {
        var cacheKey = $"device:{id}";
        var cached = await _cache.GetAsync<DeviceResponse>(cacheKey);
        
        if (cached != null)
            return cached;

        var device = await _repository.GetByIdAsync(id);
        if (device == null)
            return null;

        var response = _mapper.Map<DeviceResponse>(device);
        await _cache.SetAsync(cacheKey, response, SingleDeviceTtl);
        
        return response;
    }

    public async Task<PaginatedResponse<DeviceResponse>> GetAllAsync(int pageNumber, int pageSize)
    {
        var cacheKey = $"devices:all:{pageNumber}:{pageSize}";
        var cached = await _cache.GetAsync<PaginatedResponse<DeviceResponse>>(cacheKey);
        
        if (cached != null)
            return cached;

        var (items, totalCount) = await _repository.GetAllAsync(pageNumber, pageSize);
        var response = CreatePaginatedResponse(items, totalCount, pageNumber, pageSize);
        
        await _cache.SetAsync(cacheKey, response, AllDevicesTtl);
        return response;
    }

    public async Task<PaginatedResponse<DeviceResponse>> GetByBrandAsync(string brand, int pageNumber, int pageSize)
    {
        var cacheKey = $"devices:brand:{brand}:{pageNumber}:{pageSize}";
        var cached = await _cache.GetAsync<PaginatedResponse<DeviceResponse>>(cacheKey);
        
        if (cached != null)
            return cached;

        var (items, totalCount) = await _repository.GetByBrandAsync(brand, pageNumber, pageSize);
        var response = CreatePaginatedResponse(items, totalCount, pageNumber, pageSize);
        
        await _cache.SetAsync(cacheKey, response, CollectionTtl);
        return response;
    }

    public async Task<PaginatedResponse<DeviceResponse>> GetByStateAsync(DeviceState state, int pageNumber, int pageSize)
    {
        var cacheKey = $"devices:state:{state}:{pageNumber}:{pageSize}";
        var cached = await _cache.GetAsync<PaginatedResponse<DeviceResponse>>(cacheKey);
        
        if (cached != null)
            return cached;

        var (items, totalCount) = await _repository.GetByStateAsync(state, pageNumber, pageSize);
        var response = CreatePaginatedResponse(items, totalCount, pageNumber, pageSize);
        
        await _cache.SetAsync(cacheKey, response, CollectionTtl);
        return response;
    }

    public async Task<DeviceResponse> CreateAsync(CreateDeviceRequest request, string createdBy)
    {
        var device = _mapper.Map<Device>(request);
        device.CreatedBy = createdBy;
        
        var created = await _repository.CreateAsync(device);
        
        // Invalidate collection caches
        await InvalidateCollectionCaches();
        
        return _mapper.Map<DeviceResponse>(created);
    }

    public async Task<DeviceResponse> UpdateAsync(Guid id, UpdateDeviceRequest request)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            throw new KeyNotFoundException($"Device with ID {id} not found");

        // Business rule: Cannot update Name or Brand if device is InUse
        if (existing.State == DeviceState.InUse)
        {
            if (existing.Name != request.Name || existing.Brand != request.Brand)
            {
                throw new DomainException("Cannot update Name or Brand of a device that is currently in use");
            }
        }

        _mapper.Map(request, existing);
        var updated = await _repository.UpdateAsync(existing);
        
        // Invalidate caches
        await _cache.RemoveAsync($"device:{id}");
        await InvalidateCollectionCaches();
        
        return _mapper.Map<DeviceResponse>(updated);
    }

    public async Task<DeviceResponse> PatchAsync(Guid id, JsonPatchDocument<DevicePatchDto> patchDoc)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            throw new KeyNotFoundException($"Device with ID {id} not found");

        // Validate patch operations
        ValidatePatchOperations(patchDoc, existing);

        var patchDto = _mapper.Map<DevicePatchDto>(existing);
        patchDoc.ApplyTo(patchDto);

        // Business rule: Cannot update Name or Brand if device is InUse
        if (existing.State == DeviceState.InUse)
        {
            if ((patchDto.Name != null && patchDto.Name != existing.Name) ||
                (patchDto.Brand != null && patchDto.Brand != existing.Brand))
            {
                throw new DomainException("Cannot update Name or Brand of a device that is currently in use");
            }
        }

        // Apply changes
        if (patchDto.Name != null) existing.Name = patchDto.Name;
        if (patchDto.Brand != null) existing.Brand = patchDto.Brand;
        if (patchDto.State.HasValue) existing.State = patchDto.State.Value;

        var updated = await _repository.UpdateAsync(existing);
        
        // Invalidate caches
        await _cache.RemoveAsync($"device:{id}");
        await InvalidateCollectionCaches();
        
        return _mapper.Map<DeviceResponse>(updated);
    }

    public async Task DeleteAsync(Guid id)
    {
        var device = await _repository.GetByIdAsync(id);
        if (device == null)
            throw new KeyNotFoundException($"Device with ID {id} not found");

        // Business rule: Cannot delete device that is InUse
        if (device.State == DeviceState.InUse)
        {
            throw new DeviceInUseException();
        }

        await _repository.DeleteAsync(id);
        
        // Invalidate caches
        await _cache.RemoveAsync($"device:{id}");
        await InvalidateCollectionCaches();
    }

    private void ValidatePatchOperations(JsonPatchDocument<DevicePatchDto> patchDoc, Device existing)
    {
        var forbiddenPaths = new[] { "/id", "/creationtime", "/createdby", "/rowversion" };
        
        foreach (var operation in patchDoc.Operations)
        {
            var path = operation.path.ToLower();
            if (forbiddenPaths.Any(p => path.Contains(p)))
            {
                throw new InvalidPatchOperationException(
                    $"Cannot patch field '{operation.path}'. This field is read-only.");
            }
        }
    }

    private async Task InvalidateCollectionCaches()
    {
        await _cache.RemoveByPatternAsync("devices:*");
    }

    private PaginatedResponse<DeviceResponse> CreatePaginatedResponse(
        IEnumerable<Device> items, int totalCount, int pageNumber, int pageSize)
    {
        var deviceResponses = _mapper.Map<IEnumerable<DeviceResponse>>(items);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        
        return new PaginatedResponse<DeviceResponse>(
            deviceResponses,
            totalCount,
            pageNumber,
            pageSize,
            totalPages,
            pageNumber < totalPages,
            pageNumber > 1
        );
    }
}
