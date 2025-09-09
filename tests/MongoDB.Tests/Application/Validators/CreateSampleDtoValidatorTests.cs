using FluentAssertions;
using MongoDB.Application.DTOs;
using MongoDB.Application.Validators;
using Xunit;

namespace MongoDB.Tests.Application.Validators;

public class CreateSampleDtoValidatorTests
{
    private readonly CreateSampleDtoValidator _validator;

    public CreateSampleDtoValidatorTests()
    {
        _validator = new CreateSampleDtoValidator();
    }

    [Fact]
    public void Validate_WithValidDto_ShouldPass()
    {
        // Arrange
        var dto = new CreateSampleDto
        {
            Name = "Test Sample",
            Date = DateTime.UtcNow,
            Hash = 123.456,
            LC_FC = 789,
            Sample = new List<CreateSampleDataDto>
            {
                new()
                {
                    Data = 42.5f,
                    VahicleDate = DateTime.UtcNow,
                    RecievedDate = DateTime.UtcNow
                }
            }
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
        var dto = new CreateSampleDto
        {
            Name = string.Empty,
            Date = DateTime.UtcNow,
            Hash = 123.456,
            LC_FC = 789,
            Sample = new List<CreateSampleDataDto>()
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Name is required");
    }

    [Fact]
    public void Validate_WithTooLongName_ShouldFail()
    {
        // Arrange
        var dto = new CreateSampleDto
        {
            Name = new string('A', 101), // 101 characters
            Date = DateTime.UtcNow,
            Hash = 123.456,
            LC_FC = 789,
            Sample = new List<CreateSampleDataDto>()
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Name must not exceed 100 characters");
    }

    [Fact]
    public void Validate_WithNegativeLcFc_ShouldFail()
    {
        // Arrange
        var dto = new CreateSampleDto
        {
            Name = "Test Sample",
            Date = DateTime.UtcNow,
            Hash = 123.456,
            LC_FC = -1,
            Sample = new List<CreateSampleDataDto>()
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "LC_FC must be greater than or equal to 0");
    }

    [Fact]
    public void Validate_WithNullSampleData_ShouldFail()
    {
        // Arrange
        var dto = new CreateSampleDto
        {
            Name = "Test Sample",
            Date = DateTime.UtcNow,
            Hash = 123.456,
            LC_FC = 789,
            Sample = null!
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Sample data is required");
    }

    [Fact]
    public void Validate_WithInvalidSampleDataItem_ShouldFail()
    {
        // Arrange
        var dto = new CreateSampleDto
        {
            Name = "Test Sample",
            Date = DateTime.UtcNow,
            Hash = 123.456,
            LC_FC = 789,
            Sample = new List<CreateSampleDataDto>
            {
                new()
                {
                    Data = 42.5f,
                    VahicleDate = default, // Invalid date
                    RecievedDate = DateTime.UtcNow
                }
            }
        };

        // Act
        var result = _validator.Validate(dto);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Vehicle Date is required");
    }
}