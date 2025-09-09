using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MongoDB.Application.Mappings;
using MongoDB.Application.Services;
using MongoDB.Domain.Entities;
using MongoDB.Domain.Interfaces;
using Moq;
using Xunit;

namespace MongoDB.Tests.Application.Services;

public class SignalRClientServiceTests
{
    private readonly Mock<ISignalRClientRepository> _mockRepository;
    private readonly Mock<ILogger<SignalRClientService>> _mockLogger;
    private readonly IMapper _mapper;
    private readonly SignalRClientService _service;

    public SignalRClientServiceTests()
    {
        _mockRepository = new Mock<ISignalRClientRepository>();
        _mockLogger = new Mock<ILogger<SignalRClientService>>();
        
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();
        
        _service = new SignalRClientService(_mockRepository.Object, _mapper, _mockLogger.Object);
    }

    [Fact]
    public async Task RegisterClientAsync_WhenNewClient_ShouldCreateNewClient()
    {
        // Arrange
        var connectionId = "test-connection-123";
        var clientName = "TestClient";

        _mockRepository.Setup(r => r.GetByNameAsync(clientName))
            .ReturnsAsync((SignalRClient?)null);

        var createdClient = new SignalRClient
        {
            Id = "new-id",
            ConnectionId = connectionId,
            Name = clientName,
            IsActive = true,
            LastActivity = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<SignalRClient>(), It.IsAny<string>()))
            .ReturnsAsync(createdClient);

        // Act
        var result = await _service.RegisterClientAsync(connectionId, clientName);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be(clientName);
        result.Data.ConnectionId.Should().Be(connectionId);
        result.Message.Should().Be("Client registered successfully");
    }

    [Fact]
    public async Task RegisterClientAsync_WhenExistingClient_ShouldUpdateClient()
    {
        // Arrange
        var connectionId = "test-connection-123";
        var clientName = "TestClient";

        var existingClient = new SignalRClient
        {
            Id = "existing-id",
            ConnectionId = "old-connection",
            Name = clientName,
            IsActive = false,
            LastActivity = DateTime.UtcNow.AddMinutes(-10)
        };

        _mockRepository.Setup(r => r.GetByNameAsync(clientName))
            .ReturnsAsync(existingClient);

        var updatedClient = new SignalRClient
        {
            Id = existingClient.Id,
            ConnectionId = connectionId,
            Name = clientName,
            IsActive = true,
            LastActivity = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<SignalRClient>(), It.IsAny<string>()))
            .ReturnsAsync(updatedClient);

        // Act
        var result = await _service.RegisterClientAsync(connectionId, clientName);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.ConnectionId.Should().Be(connectionId);
        result.Data.IsActive.Should().BeTrue();
        result.Message.Should().Be("Client reconnected successfully");
    }

    [Fact]
    public async Task UnregisterClientAsync_WhenClientExists_ShouldDeactivateClient()
    {
        // Arrange
        var connectionId = "test-connection-123";

        _mockRepository.Setup(r => r.DeactivateClientAsync(connectionId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.UnregisterClientAsync(connectionId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Client unregistered successfully");
    }

    [Fact]
    public async Task UnregisterClientAsync_WhenClientDoesNotExist_ShouldReturnError()
    {
        // Arrange
        var connectionId = "non-existent-connection";

        _mockRepository.Setup(r => r.DeactivateClientAsync(connectionId))
            .ReturnsAsync(false);

        // Act
        var result = await _service.UnregisterClientAsync(connectionId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Client not found");
    }

    [Fact]
    public async Task GetActiveClientsAsync_ShouldReturnActiveClients()
    {
        // Arrange
        var activeClients = new List<SignalRClient>
        {
            new() { Id = "1", Name = "Client1", IsActive = true },
            new() { Id = "2", Name = "Client2", IsActive = true }
        };

        _mockRepository.Setup(r => r.GetActiveClientsAsync())
            .ReturnsAsync(activeClients);

        // Act
        var result = await _service.GetActiveClientsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull().And.HaveCount(2);
        result.Data!.All(c => c.IsActive).Should().BeTrue();
    }

    [Fact]
    public async Task GetClientByNameAsync_WhenClientExists_ShouldReturnClient()
    {
        // Arrange
        var clientName = "TestClient";
        var client = new SignalRClient
        {
            Id = "test-id",
            Name = clientName,
            ConnectionId = "test-connection",
            IsActive = true
        };

        _mockRepository.Setup(r => r.GetByNameAsync(clientName))
            .ReturnsAsync(client);

        // Act
        var result = await _service.GetClientByNameAsync(clientName);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be(clientName);
    }

    [Fact]
    public async Task UpdateLastActivityAsync_WhenClientExists_ShouldUpdateActivity()
    {
        // Arrange
        var connectionId = "test-connection-123";

        _mockRepository.Setup(r => r.UpdateLastActivityAsync(connectionId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.UpdateLastActivityAsync(connectionId);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Client activity updated");
    }
}