using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Application;
using MongoDB.Domain;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace MongoDB.Infrastructure;

public sealed class MongoDbSettings
{
	public string ConnectionString { get; init; } = "mongodb://localhost:27017";
	public string DatabaseName { get; init; } = "SamplesDb";
}

public interface IMongoDbContext
{
	IMongoCollection<TDocument> GetCollection<TDocument>(string collectionName);
}

public sealed class MongoDbContext : IMongoDbContext
{
	private readonly IMongoDatabase _database;

	static MongoDbContext()
	{
		// Register class maps to align domain with Mongo representation and sample schema
		if (!BsonClassMap.IsClassMapRegistered(typeof(Sample)))
		{
			BsonClassMap.RegisterClassMap<Sample>(cm =>
			{
				cm.AutoMap();
				cm.MapIdMember(x => x.Id)
					.SetIdGenerator(StringObjectIdGenerator.Instance)
					.SetSerializer(new StringSerializer(BsonType.ObjectId));
				cm.MapMember(x => x.Date).SetElementName("date");
				cm.MapMember(x => x.Name).SetElementName("Name");
				cm.MapMember(x => x.Hash).SetElementName("Hash");
				cm.MapMember(x => x.LC_FC).SetElementName("LC_FC");
				cm.MapMember(x => x.SamplePoints).SetElementName("sample");
			});
		}
		if (!BsonClassMap.IsClassMapRegistered(typeof(SamplePoint)))
		{
			BsonClassMap.RegisterClassMap<SamplePoint>(cm =>
			{
				cm.AutoMap();
				cm.MapMember(x => x.Data).SetElementName("Data");
				cm.MapMember(x => x.VehicleDate).SetElementName("Vahicle Date");
				cm.MapMember(x => x.ReceivedDate).SetElementName("recieved Date");
			});
		}
	}

	public MongoDbContext(IOptions<MongoDbSettings> options)
	{
		var client = new MongoClient(options.Value.ConnectionString);
		_database = client.GetDatabase(options.Value.DatabaseName);
	}

	public IMongoCollection<TDocument> GetCollection<TDocument>(string collectionName)
		=> _database.GetCollection<TDocument>(collectionName);
}

public sealed class GenericRepository<TDocument> : IRepository<TDocument>
{
	private readonly IMongoDbContext _context;

	public GenericRepository(IMongoDbContext context)
	{
		_context = context;
	}

	public async Task<TDocument?> GetByIdAsync(string collectionName, string id, CancellationToken cancellationToken = default)
	{
		var collection = _context.GetCollection<TDocument>(collectionName);
		var filter = ObjectId.TryParse(id, out var objectId)
			? Builders<TDocument>.Filter.Eq("_id", objectId)
			: Builders<TDocument>.Filter.Eq("_id", id);
		return await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
	}

	public async Task<IReadOnlyList<TDocument>> GetAllAsync(string collectionName, CancellationToken cancellationToken = default)
	{
		var collection = _context.GetCollection<TDocument>(collectionName);
		return await collection.Find(Builders<TDocument>.Filter.Empty).ToListAsync(cancellationToken);
	}

	public async Task<TDocument> InsertAsync(string collectionName, TDocument document, CancellationToken cancellationToken = default)
	{
		var collection = _context.GetCollection<TDocument>(collectionName);
		await collection.InsertOneAsync(document, cancellationToken: cancellationToken);
		return document;
	}

	public async Task<bool> UpdateAsync(string collectionName, string id, TDocument document, CancellationToken cancellationToken = default)
	{
		var collection = _context.GetCollection<TDocument>(collectionName);
		var filter = ObjectId.TryParse(id, out var objectId)
			? Builders<TDocument>.Filter.Eq("_id", objectId)
			: Builders<TDocument>.Filter.Eq("_id", id);
		var result = await collection.ReplaceOneAsync(filter, document, cancellationToken: cancellationToken);
		return result.IsAcknowledged && result.ModifiedCount > 0;
	}

	public async Task<bool> DeleteAsync(string collectionName, string id, CancellationToken cancellationToken = default)
	{
		var collection = _context.GetCollection<TDocument>(collectionName);
		var filter = ObjectId.TryParse(id, out var objectId)
			? Builders<TDocument>.Filter.Eq("_id", objectId)
			: Builders<TDocument>.Filter.Eq("_id", id);
		var result = await collection.DeleteOneAsync(filter, cancellationToken);
		return result.IsAcknowledged && result.DeletedCount > 0;
	}
}

public sealed class SampleRepository : ISampleRepository
{
	private const string CollectionName = "samples";
	private readonly IMongoDbContext _context;

	public SampleRepository(IMongoDbContext context)
	{
		_context = context;
	}

	public async Task<Sample?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
		=> await _context.GetCollection<Sample>(CollectionName)
			.Find(ObjectId.TryParse(id, out var objectId) ? Builders<Sample>.Filter.Eq("_id", objectId) : Builders<Sample>.Filter.Eq("_id", id))
			.FirstOrDefaultAsync(cancellationToken);

	public async Task<IReadOnlyList<Sample>> GetAllAsync(CancellationToken cancellationToken = default)
		=> await _context.GetCollection<Sample>(CollectionName).Find(Builders<Sample>.Filter.Empty).ToListAsync(cancellationToken);

	public async Task<Sample> InsertAsync(Sample sample, CancellationToken cancellationToken = default)
	{
		await _context.GetCollection<Sample>(CollectionName).InsertOneAsync(sample, cancellationToken: cancellationToken);
		return sample;
	}

	public async Task<bool> UpdateAsync(string id, Sample sample, CancellationToken cancellationToken = default)
	{
		var filter = ObjectId.TryParse(id, out var objectId) ? Builders<Sample>.Filter.Eq("_id", objectId) : Builders<Sample>.Filter.Eq("_id", id);
		var result = await _context.GetCollection<Sample>(CollectionName)
			.ReplaceOneAsync(filter, sample, cancellationToken: cancellationToken);
		return result.IsAcknowledged && result.ModifiedCount > 0;
	}

	public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
	{
		var filter = ObjectId.TryParse(id, out var objectId) ? Builders<Sample>.Filter.Eq("_id", objectId) : Builders<Sample>.Filter.Eq("_id", id);
		var result = await _context.GetCollection<Sample>(CollectionName)
			.DeleteOneAsync(filter, cancellationToken);
		return result.IsAcknowledged && result.DeletedCount > 0;
	}
}

public sealed class InMemoryClientRegistry : IClientRegistry
{
	private readonly ConcurrentDictionary<string, string> _connectionIdToName = new();

	public Task RegisterClientAsync(string name, string connectionId, CancellationToken cancellationToken = default)
	{
		_connectionIdToName[connectionId] = name;
		return Task.CompletedTask;
	}

	public Task UnregisterClientAsync(string connectionId, CancellationToken cancellationToken = default)
	{
		_connectionIdToName.TryRemove(connectionId, out _);
		return Task.CompletedTask;
	}

	public Task<IReadOnlyDictionary<string, string>> GetRegisteredClientsAsync(CancellationToken cancellationToken = default)
		=> Task.FromResult((IReadOnlyDictionary<string, string>)_connectionIdToName);
}

// SignalR hub and notifier are implemented in API layer to avoid ASP.NET refs here.

public sealed class MongoChangeStreamBackgroundService : BackgroundService
{
	private readonly ILogger<MongoChangeStreamBackgroundService> _logger;
	private readonly IMongoDbContext _context;
	private readonly ISampleAddedNotifier _notifier;

	public MongoChangeStreamBackgroundService(
		ILogger<MongoChangeStreamBackgroundService> logger,
		IMongoDbContext context,
		ISampleAddedNotifier notifier)
	{
		_logger = logger;
		_context = context;
		_notifier = notifier;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		var collection = _context.GetCollection<Sample>("samples");
		var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<Sample>>()
			.Match(x => x.OperationType == ChangeStreamOperationType.Insert);

		using var cursor = await collection.WatchAsync(pipeline, cancellationToken: stoppingToken);
		while (!stoppingToken.IsCancellationRequested && await cursor.MoveNextAsync(stoppingToken))
		{
			foreach (var change in cursor.Current)
			{
				try
				{
					var fullDocument = change.FullDocument;
					if (fullDocument is null) continue;
					var dto = new SampleDto(
						fullDocument.Id,
						fullDocument.Date,
						fullDocument.Name,
						fullDocument.Hash,
						fullDocument.LC_FC,
						MapPoints(fullDocument.SamplePoints));
					await _notifier.NotifySampleAddedAsync(dto, stoppingToken);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error processing MongoDB change stream event");
				}
			}
		}
	}

	private static IReadOnlyList<SamplePointDto> MapPoints(IReadOnlyList<SamplePoint> points)
	{
		var list = new List<SamplePointDto>(points.Count);
		foreach (var p in points)
		{
			list.Add(new SamplePointDto(p.Data, p.VehicleDate, p.ReceivedDate));
		}
		return list;
	}
}
