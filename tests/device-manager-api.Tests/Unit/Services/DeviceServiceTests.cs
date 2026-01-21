using AutoMapper;
using device_manager_api.Application.DTOs.Requests;
using device_manager_api.Application.DTOs.Responses;
using device_manager_api.Application.Exceptions;
using device_manager_api.Application.Interfaces;
using device_manager_api.Application.Services;
using device_manager_api.Domain.Entities;
using device_manager_api.Domain.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace device_manager_api.Tests.Unit.Services;

public class DeviceServiceTests
{
    private readonly Mock<IDeviceRepository> _repositoryMock;
    private readonly Mock<ICacheService> _cacheMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly DeviceService _service;

    public DeviceServiceTests()
    {
        _repositoryMock = new Mock<IDeviceRepository>();
        _cacheMock = new Mock<ICacheService>();
        _mapperMock = new Mock<IMapper>();
        _service = new DeviceService(_repositoryMock.Object, _cacheMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateDevice_AndInvalidateCache()
    {
        // Arrange
        var request = new CreateDeviceRequest("iPhone 15", "Apple", DeviceState.Available);
        var device = new Device { Id = Guid.NewGuid(), Name = "iPhone 15", Brand = "Apple", State = DeviceState.Available };
        var response = new DeviceResponse(device.Id, device.Name, device.Brand, device.State, DateTime.UtcNow, "admin1", null, null);

        _mapperMock.Setup(m => m.Map<Device>(request)).Returns(device);
        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Device>())).ReturnsAsync(device);
        _mapperMock.Setup(m => m.Map<DeviceResponse>(device)).Returns(response);
        _cacheMock.Setup(c => c.RemoveByPatternAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateAsync(request, "admin1");

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("iPhone 15");
        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Device>()), Times.Once);
        _cacheMock.Verify(c => c.RemoveByPatternAsync("devices:*"), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowException_WhenDeviceIsInUse()
    {
        // Arrange
        var deviceId = Guid.NewGuid();
        var device = new Device { Id = deviceId, State = DeviceState.InUse };
        _repositoryMock.Setup(r => r.GetByIdAsync(deviceId)).ReturnsAsync(device);

        // Act & Assert
        await Assert.ThrowsAsync<DeviceInUseException>(() => _service.DeleteAsync(deviceId));
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteDevice_WhenNotInUse()
    {
        // Arrange
        var deviceId = Guid.NewGuid();
        var device = new Device { Id = deviceId, State = DeviceState.Available };
        _repositoryMock.Setup(r => r.GetByIdAsync(deviceId)).ReturnsAsync(device);
        _repositoryMock.Setup(r => r.DeleteAsync(deviceId)).Returns(Task.CompletedTask);
        _cacheMock.Setup(c => c.RemoveAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
        _cacheMock.Setup(c => c.RemoveByPatternAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(deviceId);

        // Assert
        _repositoryMock.Verify(r => r.DeleteAsync(deviceId), Times.Once);
        _cacheMock.Verify(c => c.RemoveAsync($"device:{deviceId}"), Times.Once);
        _cacheMock.Verify(c => c.RemoveByPatternAsync("devices:*"), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCachedDevice_WhenCacheHit()
    {
        // Arrange
        var deviceId = Guid.NewGuid();
        var cachedResponse = new DeviceResponse(deviceId, "iPhone", "Apple", DeviceState.Available, DateTime.UtcNow, "admin1", null, null);
        _cacheMock.Setup(c => c.GetAsync<DeviceResponse>($"device:{deviceId}")).ReturnsAsync(cachedResponse);

        // Act
        var result = await _service.GetByIdAsync(deviceId);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(cachedResponse);
        _repositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
    }
}
