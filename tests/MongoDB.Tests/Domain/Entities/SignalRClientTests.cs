using FluentAssertions;
using MongoDB.Domain.Entities;
using Xunit;

namespace MongoDB.Tests.Domain.Entities;

public class SignalRClientTests
{
    [Fact]
    public void SignalRClient_ShouldHaveCorrectDefaultValues()
    {
        // Arrange & Act
        var client = new SignalRClient();

        // Assert
        client.Id.Should().NotBeNullOrEmpty();
        client.ConnectionId.Should().Be(string.Empty);
        client.Name.Should().Be(string.Empty);
        client.IsActive.Should().BeTrue();
        client.LastActivity.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        client.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        client.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void SignalRClient_ShouldAllowSettingProperties()
    {
        // Arrange
        var client = new SignalRClient();
        var testConnectionId = "test-connection-123";
        var testName = "TestClient";
        var testActivity = DateTime.UtcNow.AddMinutes(-5);

        // Act
        client.ConnectionId = testConnectionId;
        client.Name = testName;
        client.IsActive = false;
        client.LastActivity = testActivity;

        // Assert
        client.ConnectionId.Should().Be(testConnectionId);
        client.Name.Should().Be(testName);
        client.IsActive.Should().BeFalse();
        client.LastActivity.Should().Be(testActivity);
    }
}