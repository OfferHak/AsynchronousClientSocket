using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MyApp.Application;
using MyApp.Domain;

namespace MyApp.Infrastructure;

public sealed class MongoContext
{
	private readonly IMongoClient _client;
	private readonly IMongoDatabase _database;
	public IMongoCollection<MyDocument> Documents { get; }

	public MongoContext(IOptions<MongoOptions> options)
	{
		MongoMappings.EnsureInitialized();
		var cfg = options.Value;
		_client = new MongoClient(cfg.ConnectionString);
		_database = _client.GetDatabase(cfg.DatabaseName);
		Documents = _database.GetCollection<MyDocument>(cfg.CollectionName);
	}

	public IMongoClient Client => _client;
}

public sealed class MyDocumentRepository : IMyDocumentRepository
{
	private readonly MongoContext _context;

	public MyDocumentRepository(MongoContext context)
	{
		_context = context;
	}

	public async Task<MyDocument?> GetLatestByNameAsync(string name, CancellationToken cancellationToken)
	{
		var filter = Builders<MyDocument>.Filter.Eq(x => x.Name, name);
		var sort = Builders<MyDocument>.Sort.Descending(d => d.Date);
		return await _context.Documents
			.Find(filter)
			.Sort(sort)
			.Limit(1)
			.FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<SampleItem?> GetLatestSampleForDocumentAsync(string documentId, CancellationToken cancellationToken)
	{
		var filter = Builders<MyDocument>.Filter.Eq(d => d.Id, documentId);
		var projection = Builders<MyDocument>.Projection.Expression(d => d.Samples);
		var samples = await _context.Documents.Find(filter).Project(projection).FirstOrDefaultAsync(cancellationToken);
		return samples?.OrderByDescending(s => s.ReceivedDate ?? DateTime.MinValue).FirstOrDefault();
	}
}

public interface IChangeStreamService
{
	Task StartAsync(CancellationToken stoppingToken);
}

public sealed class ChangeStreamService : IChangeStreamService
{
	private readonly MongoContext _context;
	private readonly IChangeNotificationDispatcher _dispatcher;

	public ChangeStreamService(MongoContext context, IChangeNotificationDispatcher dispatcher)
	{
		_context = context;
		_dispatcher = dispatcher;
	}

	public async Task StartAsync(CancellationToken stoppingToken)
	{

		var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<MyDocument>>()
			.Match(x => x.OperationType == ChangeStreamOperationType.Insert || x.OperationType == ChangeStreamOperationType.Update || x.OperationType == ChangeStreamOperationType.Replace);

		var options = new ChangeStreamOptions
		{
			FullDocument = ChangeStreamFullDocumentOption.UpdateLookup
		};

		using var cursor = await _context.Documents.WatchAsync(pipeline, options, stoppingToken);
		await cursor.ForEachAsync(async change =>
		{
			var doc = change.FullDocument;
			if (doc is null) return;
			// Notify for new samples per registered Name
			var latestSample = doc.Samples.OrderByDescending(s => s.ReceivedDate ?? DateTime.MinValue).FirstOrDefault();
			if (latestSample != null && !string.IsNullOrWhiteSpace(doc.Name))
			{
				await _dispatcher.NotifySampleAddedAsync(doc.Name, new SampleItemDto(latestSample.VehicleDate, latestSample.ReceivedDate, latestSample.Name), stoppingToken);
			}
		}, stoppingToken);
	}
}
