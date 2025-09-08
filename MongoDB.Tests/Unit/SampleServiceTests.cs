using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using MongoDB.Application;
using MongoDB.Domain;
using Xunit;

namespace MongoDB.Tests.Unit;

public sealed class SampleServiceTests
{
	[Fact]
	public async Task InsertAsync_NotifiesClients()
	{
		// Arrange
		var repo = new Mock<ISampleRepository>(MockBehavior.Strict);
		var notifier = new Mock<ISampleAddedNotifier>(MockBehavior.Strict);

		var input = new SampleDto("", DateTime.UtcNow, "n", 1.0, 0, new List<SamplePointDto>());
		var entity = new Sample { Id = "507f1f77bcf86cd799439011", Date = input.Date, Name = input.Name, Hash = input.Hash, LC_FC = input.LC_FC, SamplePoints = Array.Empty<SamplePoint>() };

		repo.Setup(r => r.InsertAsync(It.IsAny<Sample>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(entity);
		notifier.Setup(n => n.NotifySampleAddedAsync(It.Is<SampleDto>(d => d.Id == entity.Id), It.IsAny<CancellationToken>()))
			.Returns(Task.CompletedTask);

		var sut = new SampleService(repo.Object, notifier.Object);

		// Act
		var result = await sut.InsertAsync(input, CancellationToken.None);

		// Assert
		result.Id.Should().Be(entity.Id);
		repo.VerifyAll();
		notifier.VerifyAll();
	}
}

