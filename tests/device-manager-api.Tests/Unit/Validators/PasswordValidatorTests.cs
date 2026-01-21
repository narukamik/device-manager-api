using device_manager_api.Application.Validators;
using FluentAssertions;
using Xunit;

namespace device_manager_api.Tests.Unit.Validators;

public class PasswordValidatorTests
{
    [Theory]
    [InlineData("P@ssw0rd", true)]
    [InlineData("Admin@123", true)]
    [InlineData("MyP@ss123", true)]
    [InlineData("short", false)]  // Too short
    [InlineData("nouppercase1@", false)]  // No uppercase
    [InlineData("NOLOWERCASE1@", false)]  // No lowercase
    [InlineData("NoDigits@", false)]  // No digit
    [InlineData("NoSpecial123", false)]  // No special char
    [InlineData("", false)]  // Empty
    public void Validate_ShouldReturnExpectedResult(string password, bool expected)
    {
        // Act
        var result = PasswordValidator.Validate(password);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void GetRequirements_ShouldReturnDescriptiveMessage()
    {
        // Act
        var requirements = PasswordValidator.GetRequirements();

        // Assert
        requirements.Should().NotBeNullOrEmpty();
        requirements.Should().Contain("8 characters");
        requirements.Should().Contain("uppercase");
        requirements.Should().Contain("lowercase");
        requirements.Should().Contain("digit");
        requirements.Should().Contain("special character");
    }
}
