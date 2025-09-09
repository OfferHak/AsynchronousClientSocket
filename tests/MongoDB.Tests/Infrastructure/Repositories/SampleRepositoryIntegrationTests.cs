using FluentAssertions;
using Microsoft.Extensions.Options;
using MongoDB.Domain.Entities;
using MongoDB.Infrastructure.Configuration;
using MongoDB.Infrastructure.Data;
using MongoDB.Infrastructure.Repositories;
using Mongo2Go;
using Xunit;

namespace MongoDB.Tests.Infrastructure.Repositories;

public class SampleRepositoryIntegrationTests : IDisposable
{
    private readonly MongoDbRunner _mongoRunner;
    private readonly MongoDbContext _context;
    private readonly SampleRepository _repository;

    public SampleRepositoryIntegrationTests()
    {
        _mongoRunner = MongoDbRunner.Start();
        
        var settings = Options.Create(new MongoDbSettings
        {
            ConnectionString = _mongoRunner.ConnectionString,
            DatabaseName = "TestDatabase"
        });

        _context = new MongoDbContext(settings);
        _repository = new SampleRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateSample()
    {
        // Arrange
        var sample = new Sample
        {
            Name = "Test Sample",
            Date = DateTime.UtcNow,
            Hash = 123.456,
            LC_FC = 789
        };

        // Act
        var result = await _repository.CreateAsync(sample, "samples");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrEmpty();
        result.Name.Should().Be("Test Sample");
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task GetByIdAsync_WhenSampleExists_ShouldReturnSample()
    {
        // Arrange
        var sample = new Sample
        {
            Name = "Test Sample",
            Date = DateTime.UtcNow,
            Hash = 123.456,
            LC_FC = 789
        };

        var createdSample = await _repository.CreateAsync(sample, "samples");

        // Act
        var result = await _repository.GetByIdAsync(createdSample.Id, "samples");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(createdSample.Id);
        result.Name.Should().Be("Test Sample");
    }

    [Fact]
    public async Task GetByIdAsync_WhenSampleDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = "507f1f77bcf86cd799439011";

        // Act
        var result = await _repository.GetByIdAsync(nonExistentId, "samples");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetSamplesByNameAsync_ShouldReturnMatchingSamples()
    {
        // Arrange
        var sample1 = new Sample { Name = "Test Sample 1", Date = DateTime.UtcNow };
        var sample2 = new Sample { Name = "Test Sample 2", Date = DateTime.UtcNow };
        var sample3 = new Sample { Name = "Different Name", Date = DateTime.UtcNow };

        await _repository.CreateAsync(sample1, "samples");
        await _repository.CreateAsync(sample2, "samples");
        await _repository.CreateAsync(sample3, "samples");

        // Act
        var result = await _repository.GetSamplesByNameAsync("Test Sample");

        // Assert
        result.Should().HaveCount(2);
        result.All(s => s.Name.Contains("Test Sample")).Should().BeTrue();
    }

    [Fact]
    public async Task GetSamplesByDateRangeAsync_ShouldReturnSamplesInRange()
    {
        // Arrange
        var baseDate = DateTime.UtcNow.Date;
        var sample1 = new Sample { Name = "Sample 1", Date = baseDate };
        var sample2 = new Sample { Name = "Sample 2", Date = baseDate.AddDays(1) };
        var sample3 = new Sample { Name = "Sample 3", Date = baseDate.AddDays(5) };

        await _repository.CreateAsync(sample1, "samples");
        await _repository.CreateAsync(sample2, "samples");
        await _repository.CreateAsync(sample3, "samples");

        // Act
        var result = await _repository.GetSamplesByDateRangeAsync(baseDate, baseDate.AddDays(2));

        // Assert
        result.Should().HaveCount(2);
        result.All(s => s.Date >= baseDate && s.Date <= baseDate.AddDays(2)).Should().BeTrue();
    }

    [Fact]
    public async Task GetLatestSampleAsync_ShouldReturnMostRecentSample()
    {
        // Arrange
        var sample1 = new Sample { Name = "Sample 1", Date = DateTime.UtcNow };
        var sample2 = new Sample { Name = "Sample 2", Date = DateTime.UtcNow };

        await _repository.CreateAsync(sample1, "samples");
        await Task.Delay(10); // Ensure different CreatedAt times
        await _repository.CreateAsync(sample2, "samples");

        // Act
        var result = await _repository.GetLatestSampleAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Sample 2");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateSample()
    {
        // Arrange
        var sample = new Sample
        {
            Name = "Original Name",
            Date = DateTime.UtcNow,
            Hash = 123.456
        };

        var createdSample = await _repository.CreateAsync(sample, "samples");
        createdSample.Name = "Updated Name";
        createdSample.Hash = 999.888;

        // Act
        var result = await _repository.UpdateAsync(createdSample, "samples");

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Updated Name");
        result.Hash.Should().Be(999.888);
        result.UpdatedAt.Should().BeAfter(result.CreatedAt);
    }

    [Fact]
    public async Task DeleteAsync_WhenSampleExists_ShouldDeleteAndReturnTrue()
    {
        // Arrange
        var sample = new Sample
        {
            Name = "Sample to Delete",
            Date = DateTime.UtcNow
        };

        var createdSample = await _repository.CreateAsync(sample, "samples");

        // Act
        var result = await _repository.DeleteAsync(createdSample.Id, "samples");

        // Assert
        result.Should().BeTrue();

        // Verify deletion
        var deletedSample = await _repository.GetByIdAsync(createdSample.Id, "samples");
        deletedSample.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenSampleDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var nonExistentId = "507f1f77bcf86cd799439011";

        // Act
        var result = await _repository.DeleteAsync(nonExistentId, "samples");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCorrectCount()
    {
        // Arrange
        var sample1 = new Sample { Name = "Sample 1", Date = DateTime.UtcNow };
        var sample2 = new Sample { Name = "Sample 2", Date = DateTime.UtcNow };

        await _repository.CreateAsync(sample1, "samples");
        await _repository.CreateAsync(sample2, "samples");

        // Act
        var result = await _repository.CountAsync("samples");

        // Assert
        result.Should().Be(2);
    }

    public void Dispose()
    {
        _mongoRunner?.Dispose();
    }
}