using FluentAssertions;
using MongoDB.Domain.Entities;
using Xunit;

namespace MongoDB.Tests.Domain.Entities;

public class SampleTests
{
    [Fact]
    public void Sample_ShouldHaveCorrectDefaultValues()
    {
        // Arrange & Act
        var sample = new Sample();

        // Assert
        sample.Id.Should().NotBeNullOrEmpty();
        sample.Name.Should().Be(string.Empty);
        sample.SampleData.Should().NotBeNull().And.BeEmpty();
        sample.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        sample.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Sample_ShouldAllowSettingProperties()
    {
        // Arrange
        var sample = new Sample();
        var testDate = DateTime.UtcNow.Date;
        var testName = "Test Sample";
        var testHash = 123.456;
        var testLcFc = 789;

        // Act
        sample.Date = testDate;
        sample.Name = testName;
        sample.Hash = testHash;
        sample.LC_FC = testLcFc;

        // Assert
        sample.Date.Should().Be(testDate);
        sample.Name.Should().Be(testName);
        sample.Hash.Should().Be(testHash);
        sample.LC_FC.Should().Be(testLcFc);
    }

    [Fact]
    public void SampleData_ShouldAllowSettingProperties()
    {
        // Arrange
        var sampleData = new SampleData();
        var testData = 42.5f;
        var testVehicleDate = DateTime.UtcNow.Date;
        var testReceivedDate = DateTime.UtcNow.Date.AddHours(1);

        // Act
        sampleData.Data = testData;
        sampleData.VahicleDate = testVehicleDate;
        sampleData.RecievedDate = testReceivedDate;

        // Assert
        sampleData.Data.Should().Be(testData);
        sampleData.VahicleDate.Should().Be(testVehicleDate);
        sampleData.RecievedDate.Should().Be(testReceivedDate);
    }
}