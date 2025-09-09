using FluentAssertions;
using MongoDB.Application.DTOs;
using MongoDB.Application.Validators;
using Xunit;

namespace MongoDB.Tests.Application.Validators;

public class RegisterClientDtoValidatorTests
{
    private readonly RegisterClientDtoValidator _validator;

    public RegisterClientDtoValidatorTests()
    {
        _validator = new RegisterClientDtoValidator();
    }

    [Fact]
    public void Validate_WithValidDto_ShouldPass()
    {
        // Arrange
        var dto = new RegisterClientDto
        {
            Name = "TestClient123"
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Validate_WithEmptyName_ShouldFail()
    {
        // Arrange
        var dto = new RegisterClientDto
        {
            Name = string.Empty
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Name is required");
    }

    [Fact]
    public void Validate_WithTooShortName_ShouldFail()
    {
        // Arrange
        var dto = new RegisterClientDto
        {
            Name = "A" // Only 1 character
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Name must be at least 2 characters long");
    }

    [Fact]
    public void Validate_WithTooLongName_ShouldFail()
    {
        // Arrange
        var dto = new RegisterClientDto
        {
            Name = new string('A', 51) // 51 characters
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Name must not exceed 50 characters");
    }

    [Theory]
    [InlineData("Test Client")] // Contains space
    [InlineData("Test@Client")] // Contains @
    [InlineData("Test.Client")] // Contains .
    [InlineData("Test Client!")] // Contains space and !
    public void Validate_WithInvalidCharacters_ShouldFail(string invalidName)
    {
        // Arrange
        var dto = new RegisterClientDto
        {
            Name = invalidName
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Name can only contain letters, numbers, underscores, and hyphens");
    }

    [Theory]
    [InlineData("TestClient")]
    [InlineData("Test_Client")]
    [InlineData("Test-Client")]
    [InlineData("TestClient123")]
    [InlineData("Test_Client-123")]
    public void Validate_WithValidCharacters_ShouldPass(string validName)
    {
        // Arrange
        var dto = new RegisterClientDto
        {
            Name = validName
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}