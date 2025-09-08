using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.API;
using MongoDB.Domain.Entities;
using MongoDB.Domain.Interfaces;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;

namespace MongoDB.Tests.Integration;

public class SamplesControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public SamplesControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Replace with in-memory implementations for testing
                services.AddScoped<ISampleService, MockSampleService>();
                services.AddScoped<INotificationService, MockNotificationService>();
            });
        });
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetAllSamples_ReturnsOkResult()
    {
        // Act
        var response = await _client.GetAsync("/api/samples");

        // Assert
        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType!.ToString().Should().Contain("application/json");
    }

    [Fact]
    public async Task GetSampleById_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var id = "507f1f77bcf86cd799439011";

        // Act
        var response = await _client.GetAsync($"/api/samples/{id}");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetSampleById_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var id = "invalid-id";

        // Act
        var response = await _client.GetAsync($"/api/samples/{id}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateSample_WithValidData_ReturnsCreatedResult()
    {
        // Arrange
        var createDto = new
        {
            Name = "Test Sample",
            Date = DateTime.UtcNow,
            Hash = 123.45,
            LC_FC = 10,
            Sample = new[]
            {
                new
                {
                    Data = 1.5f,
                    VehicleDate = DateTime.UtcNow,
                    ReceivedDate = DateTime.UtcNow
                }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/samples", createDto);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
    }

    [Fact]
    public async Task GetSampleCount_ReturnsOkResult()
    {
        // Act
        var response = await _client.GetAsync("/api/samples/count");

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task HealthCheck_ReturnsOkResult()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Healthy");
    }
}

// Mock implementations for testing
public class MockSampleService : ISampleService
{
    private readonly List<SampleDocument> _samples = new()
    {
        new()
        {
            Id = "507f1f77bcf86cd799439011",
            Name = "Test Sample 1",
            Date = DateTime.UtcNow,
            Hash = 123.45,
            LC_FC = 10,
            Sample = new List<SampleData>()
        },
        new()
        {
            Id = "507f1f77bcf86cd799439012",
            Name = "Test Sample 2",
            Date = DateTime.UtcNow,
            Hash = 234.56,
            LC_FC = 20,
            Sample = new List<SampleData>()
        }
    };

    public Task<SampleDocument> CreateSampleAsync(SampleDocument sample)
    {
        sample.Id = "507f1f77bcf86cd799439013";
        _samples.Add(sample);
        return Task.FromResult(sample);
    }

    public Task<SampleDocument?> GetSampleByIdAsync(string id)
    {
        var sample = _samples.FirstOrDefault(s => s.Id == id);
        return Task.FromResult(sample);
    }

    public Task<IEnumerable<SampleDocument>> GetAllSamplesAsync()
    {
        return Task.FromResult<IEnumerable<SampleDocument>>(_samples);
    }

    public Task<IEnumerable<SampleDocument>> GetSamplesByNameAsync(string name)
    {
        var samples = _samples.Where(s => s.Name == name);
        return Task.FromResult(samples);
    }

    public Task<SampleDocument> UpdateSampleAsync(string id, SampleDocument sample)
    {
        var existingSample = _samples.FirstOrDefault(s => s.Id == id);
        if (existingSample != null)
        {
            existingSample.Name = sample.Name;
            existingSample.Date = sample.Date;
            existingSample.Hash = sample.Hash;
            existingSample.LC_FC = sample.LC_FC;
            existingSample.Sample = sample.Sample;
        }
        return Task.FromResult(sample);
    }

    public Task<bool> DeleteSampleAsync(string id)
    {
        var sample = _samples.FirstOrDefault(s => s.Id == id);
        if (sample != null)
        {
            _samples.Remove(sample);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<long> GetSampleCountAsync()
    {
        return Task.FromResult((long)_samples.Count);
    }

    public Task<IEnumerable<SampleDocument>> GetPagedSamplesAsync(int page, int pageSize)
    {
        var skip = (page - 1) * pageSize;
        var pagedSamples = _samples.Skip(skip).Take(pageSize);
        return Task.FromResult(pagedSamples);
    }
}

public class MockNotificationService : INotificationService
{
    private readonly Dictionary<string, string> _clients = new();

    public Task RegisterClientAsync(string clientName, string connectionId)
    {
        _clients[connectionId] = clientName;
        return Task.CompletedTask;
    }

    public Task UnregisterClientAsync(string connectionId)
    {
        _clients.Remove(connectionId);
        return Task.CompletedTask;
    }

    public Task NotifyNewSampleAsync(string clientName, object sampleData)
    {
        return Task.CompletedTask;
    }

    public Task NotifyAllClientsAsync(object data)
    {
        return Task.CompletedTask;
    }

    public Task<IEnumerable<string>> GetRegisteredClientsAsync()
    {
        return Task.FromResult<IEnumerable<string>>(_clients.Values.Distinct());
    }
}