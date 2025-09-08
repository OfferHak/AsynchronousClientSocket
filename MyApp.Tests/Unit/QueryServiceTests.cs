using FluentAssertions;
using Moq;
using MyApp.Application;
using MyApp.Domain;

namespace MyApp.Tests.Unit;

public sealed class QueryServiceTests
{
	[Fact]
	public async Task GetLatestSampleForNameAsync_ReturnsLatestByReceivedDate()
	{
		var repo = new Mock<IMyDocumentRepository>();
		repo.Setup(r => r.GetLatestByNameAsync("A", It.IsAny<CancellationToken>()))
			.ReturnsAsync(new MyDocument
			{
				Id = "1",
				Name = "A",
				Date = DateTime.UtcNow,
				Samples = new List<SampleItem>
				{
					new SampleItem{ ReceivedDate = new DateTime(2020,1,1), Name = "x" },
					new SampleItem{ ReceivedDate = new DateTime(2021,1,1), Name = "y" }
				}
			});

		var sut = new MyDocumentQueryService(repo.Object);
		var result = await sut.GetLatestSampleForNameAsync("A", CancellationToken.None);

		result.Should().NotBeNull();
		result!.Name.Should().Be("y");
	}
}

