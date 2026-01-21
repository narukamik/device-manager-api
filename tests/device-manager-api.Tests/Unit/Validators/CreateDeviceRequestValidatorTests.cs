using device_manager_api.Application.DTOs.Requests;
using device_manager_api.Application.Validators;
using device_manager_api.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace device_manager_api.Tests.Unit.Validators;

public class CreateDeviceRequestValidatorTests
{
    private readonly CreateDeviceRequestValidator _validator;

    public CreateDeviceRequestValidatorTests()
    {
        _validator = new CreateDeviceRequestValidator();
    }

    [Fact]
    public void Validate_ShouldPass_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateDeviceRequest("iPhone 15", "Apple", DeviceState.Available);

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameIsEmpty()
    {
        // Arrange
        var request = new CreateDeviceRequest("", "Apple", DeviceState.Available);

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_ShouldFail_WhenBrandIsEmpty()
    {
        // Arrange
        var request = new CreateDeviceRequest("iPhone 15", "", DeviceState.Available);

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Brand");
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameExceedsMaxLength()
    {
        // Arrange
        var longName = new string('a', 101);
        var request = new CreateDeviceRequest(longName, "Apple", DeviceState.Available);

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage.Contains("100"));
    }

    [Fact]
    public void Validate_ShouldFail_WhenBrandExceedsMaxLength()
    {
        // Arrange
        var longBrand = new string('a', 51);
        var request = new CreateDeviceRequest("iPhone 15", longBrand, DeviceState.Available);

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Brand" && e.ErrorMessage.Contains("50"));
    }
}
