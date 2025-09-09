using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Application.DTOs;
using MongoDB.Application.Mappings;
using MongoDB.Application.Services;
using MongoDB.Domain.Entities;
using MongoDB.Domain.Interfaces;
using Moq;
using Xunit;

namespace MongoDB.Tests.Application.Services;

public class SampleServiceTests
{
    private readonly Mock<ISampleRepository> _mockRepository;
    private readonly Mock<INotificationService> _mockNotificationService;
    private readonly Mock<ILogger<SampleService>> _mockLogger;
    private readonly IMapper _mapper;
    private readonly SampleService _service;

    public SampleServiceTests()
    {
        _mockRepository = new Mock<ISampleRepository>();
        _mockNotificationService = new Mock<INotificationService>();
        _mockLogger = new Mock<ILogger<SampleService>>();
        
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        
        _service = new SampleService(_mockRepository.Object, _mapper, _mockLogger.Object, _mockNotificationService.Object);
    }

    [Fact]
    public async Task GetAllSamplesAsync_ShouldReturnPaginatedResult()
    {
        // Arrange
        var samples = new List<Sample>
        {
            new() { Id = "1", Name = "Sample 1", Date = DateTime.UtcNow },
            new() { Id = "2", Name = "Sample 2", Date = DateTime.UtcNow }
        };

        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<string>()))
            .ReturnsAsync(samples);
        _mockRepository.Setup(r => r.CountAsync(It.IsAny<string>()))
            .ReturnsAsync(2);

        // Act
        var result = await _service.GetAllSamplesAsync(1, 10);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Items.Should().HaveCount(2);
        result.Data.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task GetSampleByIdAsync_WhenSampleExists_ShouldReturnSample()
    {
        // Arrange
        var sampleId = "test-id";
        var sample = new Sample { Id = sampleId, Name = "Test Sample", Date = DateTime.UtcNow };

        _mockRepository.Setup(r => r.GetByIdAsync(sampleId, It.IsAny<string>()))
            .ReturnsAsync(sample);

        // Act
        var result = await _service.GetSampleByIdAsync(sampleId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(sampleId);
        result.Data.Name.Should().Be("Test Sample");
    }

    [Fact]
    public async Task GetSampleByIdAsync_WhenSampleDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var sampleId = "non-existent-id";

        _mockRepository.Setup(r => r.GetByIdAsync(sampleId, It.IsAny<string>()))
            .ReturnsAsync((Sample?)null);

        // Act
        var result = await _service.GetSampleByIdAsync(sampleId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Sample not found");
        result.Data.Should().BeNull();
    }

    [Fact]
    public async Task CreateSampleAsync_ShouldCreateSampleAndNotifyClients()
    {
        // Arrange
        var createDto = new CreateSampleDto
        {
            Name = "New Sample",
            Date = DateTime.UtcNow,
            Hash = 123.456,
            LC_FC = 789,
            Sample = new List<CreateSampleDataDto>
            {
                new() { Data = 42.5f, VahicleDate = DateTime.UtcNow, RecievedDate = DateTime.UtcNow }
            }
        };

        var createdSample = new Sample
        {
            Id = "new-id",
            Name = createDto.Name,
            Date = createDto.Date,
            Hash = createDto.Hash,
            LC_FC = createDto.LC_FC
        };

        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Sample>(), It.IsAny<string>()))
            .ReturnsAsync(createdSample);

        // Act
        var result = await _service.CreateSampleAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("New Sample");
        
        _mockNotificationService.Verify(n => n.NotifyNewSampleAsync(It.IsAny<SampleDto>()), Times.Once);
    }

    [Fact]
    public async Task UpdateSampleAsync_WhenSampleExists_ShouldUpdateSample()
    {
        // Arrange
        var sampleId = "existing-id";
        var updateDto = new UpdateSampleDto
        {
            Id = sampleId,
            Name = "Updated Sample",
            Date = DateTime.UtcNow,
            Hash = 999.888,
            LC_FC = 555
        };

        var existingSample = new Sample
        {
            Id = sampleId,
            Name = "Original Sample",
            Date = DateTime.UtcNow.AddDays(-1),
            CreatedAt = DateTime.UtcNow.AddDays(-1)
        };

        var updatedSample = new Sample
        {
            Id = sampleId,
            Name = updateDto.Name,
            Date = updateDto.Date,
            Hash = updateDto.Hash,
            LC_FC = updateDto.LC_FC,
            CreatedAt = existingSample.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.GetByIdAsync(sampleId, It.IsAny<string>()))
            .ReturnsAsync(existingSample);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Sample>(), It.IsAny<string>()))
            .ReturnsAsync(updatedSample);

        // Act
        var result = await _service.UpdateSampleAsync(updateDto);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Updated Sample");
    }

    [Fact]
    public async Task DeleteSampleAsync_WhenSampleExists_ShouldDeleteSample()
    {
        // Arrange
        var sampleId = "existing-id";

        _mockRepository.Setup(r => r.DeleteAsync(sampleId, It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        var result = await _service.DeleteSampleAsync(sampleId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Sample deleted successfully");
    }

    [Fact]
    public async Task DeleteSampleAsync_WhenSampleDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var sampleId = "non-existent-id";

        _mockRepository.Setup(r => r.DeleteAsync(sampleId, It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        var result = await _service.DeleteSampleAsync(sampleId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Sample not found");
    }
}