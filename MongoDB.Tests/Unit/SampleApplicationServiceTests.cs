using AutoMapper;
using FluentAssertions;
using MongoDB.Application.DTOs;
using MongoDB.Application.Mappings;
using MongoDB.Application.Services;
using MongoDB.Application.Validators;
using MongoDB.Domain.Entities;
using MongoDB.Domain.Interfaces;
using Moq;

namespace MongoDB.Tests.Unit;

public class SampleApplicationServiceTests
{
    private readonly Mock<ISampleService> _mockSampleService;
    private readonly IMapper _mapper;
    private readonly CreateSampleDtoValidator _createValidator;
    private readonly UpdateSampleDtoValidator _updateValidator;
    private readonly SampleApplicationService _service;

    public SampleApplicationServiceTests()
    {
        _mockSampleService = new Mock<ISampleService>();
        
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        
        _createValidator = new CreateSampleDtoValidator();
        _updateValidator = new UpdateSampleDtoValidator();
        
        _service = new SampleApplicationService(
            _mockSampleService.Object,
            _mapper,
            _createValidator,
            _updateValidator);
    }

    [Fact]
    public async Task CreateSampleAsync_ValidData_ReturnsSampleDto()
    {
        // Arrange
        var createDto = new CreateSampleDto
        {
            Name = "Test Sample",
            Date = DateTime.UtcNow,
            Hash = 123.45,
            LC_FC = 10,
            Sample = new List<SampleDataDto>
            {
                new() { Data = 1.5f, VehicleDate = DateTime.UtcNow, ReceivedDate = DateTime.UtcNow }
            }
        };

        var sample = new SampleDocument
        {
            Id = "507f1f77bcf86cd799439011",
            Name = createDto.Name,
            Date = createDto.Date,
            Hash = createDto.Hash,
            LC_FC = createDto.LC_FC,
            Sample = new List<SampleData>
            {
                new() { Data = 1.5f, VehicleDate = DateTime.UtcNow, ReceivedDate = DateTime.UtcNow }
            }
        };

        _mockSampleService.Setup(x => x.CreateSampleAsync(It.IsAny<SampleDocument>()))
                         .ReturnsAsync(sample);

        // Act
        var result = await _service.CreateSampleAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(createDto.Name);
        result.Hash.Should().Be(createDto.Hash);
        result.LC_FC.Should().Be(createDto.LC_FC);
        result.Sample.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetSampleByIdAsync_ExistingId_ReturnsSampleDto()
    {
        // Arrange
        var id = "507f1f77bcf86cd799439011";
        var sample = new SampleDocument
        {
            Id = id,
            Name = "Test Sample",
            Date = DateTime.UtcNow,
            Hash = 123.45,
            LC_FC = 10,
            Sample = new List<SampleData>()
        };

        _mockSampleService.Setup(x => x.GetSampleByIdAsync(id))
                         .ReturnsAsync(sample);

        // Act
        var result = await _service.GetSampleByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.Name.Should().Be("Test Sample");
    }

    [Fact]
    public async Task GetSampleByIdAsync_NonExistingId_ReturnsNull()
    {
        // Arrange
        var id = "507f1f77bcf86cd799439011";
        _mockSampleService.Setup(x => x.GetSampleByIdAsync(id))
                         .ReturnsAsync((SampleDocument?)null);

        // Act
        var result = await _service.GetSampleByIdAsync(id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllSamplesAsync_ReturnsAllSamples()
    {
        // Arrange
        var samples = new List<SampleDocument>
        {
            new() { Id = "1", Name = "Sample 1", Date = DateTime.UtcNow, Hash = 1.0, LC_FC = 1, Sample = new List<SampleData>() },
            new() { Id = "2", Name = "Sample 2", Date = DateTime.UtcNow, Hash = 2.0, LC_FC = 2, Sample = new List<SampleData>() }
        };

        _mockSampleService.Setup(x => x.GetAllSamplesAsync())
                         .ReturnsAsync(samples);

        // Act
        var result = await _service.GetAllSamplesAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(s => s.Should().NotBeNull());
    }

    [Fact]
    public async Task DeleteSampleAsync_ExistingId_ReturnsTrue()
    {
        // Arrange
        var id = "507f1f77bcf86cd799439011";
        _mockSampleService.Setup(x => x.DeleteSampleAsync(id))
                         .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteSampleAsync(id);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteSampleAsync_NonExistingId_ReturnsFalse()
    {
        // Arrange
        var id = "507f1f77bcf86cd799439011";
        _mockSampleService.Setup(x => x.DeleteSampleAsync(id))
                         .ReturnsAsync(false);

        // Act
        var result = await _service.DeleteSampleAsync(id);

        // Assert
        result.Should().BeFalse();
    }
}