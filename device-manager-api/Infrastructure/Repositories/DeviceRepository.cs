using device_manager_api.Application.Exceptions;
using device_manager_api.Application.Interfaces;
using device_manager_api.Domain.Entities;
using device_manager_api.Domain.Enums;
using device_manager_api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace device_manager_api.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Device operations
/// </summary>
public class DeviceRepository : IDeviceRepository
{
    private readonly ApplicationDbContext _context;

    public DeviceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Device?> GetByIdAsync(Guid id)
    {
        return await _context.Devices
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<(IEnumerable<Device> Items, int TotalCount)> GetAllAsync(int pageNumber, int pageSize)
    {
        var query = _context.Devices.AsNoTracking();
        var totalCount = await query.CountAsync();
        
        var items = await query
            .OrderBy(d => d.CreationTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<(IEnumerable<Device> Items, int TotalCount)> GetByBrandAsync(string brand, int pageNumber, int pageSize)
    {
        var query = _context.Devices
            .AsNoTracking()
            .Where(d => d.Brand == brand);
        
        var totalCount = await query.CountAsync();
        
        var items = await query
            .OrderBy(d => d.CreationTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<(IEnumerable<Device> Items, int TotalCount)> GetByStateAsync(DeviceState state, int pageNumber, int pageSize)
    {
        var query = _context.Devices
            .AsNoTracking()
            .Where(d => d.State == state);
        
        var totalCount = await query.CountAsync();
        
        var items = await query
            .OrderBy(d => d.CreationTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Device> CreateAsync(Device device)
    {
        device.Id = Guid.NewGuid();
        device.CreationTime = DateTime.UtcNow;
        
        _context.Devices.Add(device);
        await _context.SaveChangesAsync();
        
        return device;
    }

    public async Task<Device> UpdateAsync(Device device)
    {
        try
        {
            _context.Devices.Update(device);
            await _context.SaveChangesAsync();
            return device;
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException();
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var device = await _context.Devices.FindAsync(id);
        if (device != null)
        {
            _context.Devices.Remove(device);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Devices.AnyAsync(d => d.Id == id);
    }
}
