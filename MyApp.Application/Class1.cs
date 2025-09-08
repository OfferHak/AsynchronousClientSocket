using System.ComponentModel.DataAnnotations;
using MyApp.Domain;

namespace MyApp.Application;

public sealed class MongoOptions
{
	[Required]
	public string ConnectionString { get; set; } = string.Empty;

	[Required]
	public string DatabaseName { get; set; } = string.Empty;

	[Required]
	public string CollectionName { get; set; } = "documents";
}

public sealed record SampleItemDto(DateTime? VehicleDate, DateTime? ReceivedDate, string? Name);

public sealed record MyDocumentDto(string? Id, DateTime? Date, string Name, string? Hash, string? LcFc, IReadOnlyList<SampleItemDto> Samples);

public interface IMyDocumentRepository
{
	Task<MyDocument?> GetLatestByNameAsync(string name, CancellationToken cancellationToken);
	Task<SampleItem?> GetLatestSampleForDocumentAsync(string documentId, CancellationToken cancellationToken);
}

public interface IMyDocumentQueryService
{
	Task<MyDocumentDto?> GetLatestDocumentByNameAsync(string name, CancellationToken cancellationToken);
	Task<SampleItemDto?> GetLatestSampleForNameAsync(string name, CancellationToken cancellationToken);
}

public sealed class MyDocumentQueryService : IMyDocumentQueryService
{
	private readonly IMyDocumentRepository _repository;

	public MyDocumentQueryService(IMyDocumentRepository repository)
	{
		_repository = repository;
	}

	public async Task<MyDocumentDto?> GetLatestDocumentByNameAsync(string name, CancellationToken cancellationToken)
	{
		var entity = await _repository.GetLatestByNameAsync(name, cancellationToken);
		return entity is null ? null : Map(entity);
	}

	public async Task<SampleItemDto?> GetLatestSampleForNameAsync(string name, CancellationToken cancellationToken)
	{
		var entity = await _repository.GetLatestByNameAsync(name, cancellationToken);
		if (entity is null)
		{
			return null;
		}
		var latest = entity.Samples
			.OrderByDescending(s => s.ReceivedDate ?? DateTime.MinValue)
			.FirstOrDefault();
		return latest is null ? null : new SampleItemDto(latest.VehicleDate, latest.ReceivedDate, latest.Name);
	}

	private static MyDocumentDto Map(MyDocument entity)
	{
		return new MyDocumentDto(
			entity.Id,
			entity.Date,
			entity.Name,
			entity.Hash,
			entity.LcFc,
			entity.Samples.Select(s => new SampleItemDto(s.VehicleDate, s.ReceivedDate, s.Name)).ToList());
	}
}

public interface INotificationSubscriptionService
{
	Task SubscribeAsync(string connectionId, string name, CancellationToken cancellationToken);
	Task UnsubscribeAsync(string connectionId, string name, CancellationToken cancellationToken);
	Task RemoveAllForConnectionAsync(string connectionId, CancellationToken cancellationToken);
}

public interface IChangeNotificationDispatcher
{
	Task NotifySampleAddedAsync(string name, SampleItemDto sample, CancellationToken cancellationToken);
}
