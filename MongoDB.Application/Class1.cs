using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Domain;
using MongoDB.Bson;

namespace MongoDB.Application;

// DTOs
public sealed record SamplePointDto(float Data, DateTime VehicleDate, DateTime ReceivedDate);
public sealed record SampleDto(string Id, DateTime Date, string Name, double Hash, int LC_FC, IReadOnlyList<SamplePointDto> Sample);

// Contracts
public interface IGenericCollectionService
{
	Task<BsonDocument?> GetByIdAsync(string collectionName, string id, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<BsonDocument>> GetAllAsync(string collectionName, CancellationToken cancellationToken = default);
	Task<BsonDocument> InsertAsync(string collectionName, BsonDocument document, CancellationToken cancellationToken = default);
	Task<bool> UpdateAsync(string collectionName, string id, BsonDocument document, CancellationToken cancellationToken = default);
	Task<bool> DeleteAsync(string collectionName, string id, CancellationToken cancellationToken = default);
}

public interface ISampleService
{
	Task<SampleDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<SampleDto>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<SampleDto> InsertAsync(SampleDto sample, CancellationToken cancellationToken = default);
	Task<bool> UpdateAsync(string id, SampleDto sample, CancellationToken cancellationToken = default);
	Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}

public sealed class GenericCollectionService : IGenericCollectionService
{
	private readonly IRepository<BsonDocument> _genericRepository;

	public GenericCollectionService(IRepository<BsonDocument> genericRepository)
	{
		_genericRepository = genericRepository;
	}

	public Task<BsonDocument?> GetByIdAsync(string collectionName, string id, CancellationToken cancellationToken = default)
		=> _genericRepository.GetByIdAsync(collectionName, id, cancellationToken);

	public Task<IReadOnlyList<BsonDocument>> GetAllAsync(string collectionName, CancellationToken cancellationToken = default)
		=> _genericRepository.GetAllAsync(collectionName, cancellationToken);

	public Task<BsonDocument> InsertAsync(string collectionName, BsonDocument document, CancellationToken cancellationToken = default)
		=> _genericRepository.InsertAsync(collectionName, document, cancellationToken);

	public Task<bool> UpdateAsync(string collectionName, string id, BsonDocument document, CancellationToken cancellationToken = default)
		=> _genericRepository.UpdateAsync(collectionName, id, document, cancellationToken);

	public Task<bool> DeleteAsync(string collectionName, string id, CancellationToken cancellationToken = default)
		=> _genericRepository.DeleteAsync(collectionName, id, cancellationToken);
}

public interface IClientRegistry
{
	Task RegisterClientAsync(string name, string connectionId, CancellationToken cancellationToken = default);
	Task UnregisterClientAsync(string connectionId, CancellationToken cancellationToken = default);
	Task<IReadOnlyDictionary<string, string>> GetRegisteredClientsAsync(CancellationToken cancellationToken = default);
}

public interface ISampleAddedNotifier
{
	Task NotifySampleAddedAsync(SampleDto sample, CancellationToken cancellationToken = default);
}

// Use cases
public sealed class SampleService : ISampleService
{
	private readonly ISampleRepository _repository;
	private readonly ISampleAddedNotifier _notifier;

	public SampleService(ISampleRepository repository, ISampleAddedNotifier notifier)
	{
		_repository = repository;
		_notifier = notifier;
	}

	public async Task<SampleDto?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
	{
		var entity = await _repository.GetByIdAsync(id, cancellationToken);
		return entity is null ? null : MapToDto(entity);
	}

	public async Task<IReadOnlyList<SampleDto>> GetAllAsync(CancellationToken cancellationToken = default)
	{
		var entities = await _repository.GetAllAsync(cancellationToken);
		var list = new List<SampleDto>(entities.Count);
		foreach (var e in entities)
		{
			list.Add(MapToDto(e));
		}
		return list;
	}

	public async Task<SampleDto> InsertAsync(SampleDto sample, CancellationToken cancellationToken = default)
	{
		var entity = MapToEntity(sample);
		var inserted = await _repository.InsertAsync(entity, cancellationToken);
		var dto = MapToDto(inserted);
		await _notifier.NotifySampleAddedAsync(dto, cancellationToken);
		return dto;
	}

	public async Task<bool> UpdateAsync(string id, SampleDto sample, CancellationToken cancellationToken = default)
	{
		var entity = MapToEntity(sample);
		return await _repository.UpdateAsync(id, entity, cancellationToken);
	}

	public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
		=> _repository.DeleteAsync(id, cancellationToken);

	private static SampleDto MapToDto(Sample entity)
	{
		var points = new List<SamplePointDto>(entity.SamplePoints.Count);
		foreach (var p in entity.SamplePoints)
		{
			points.Add(new SamplePointDto(p.Data, p.VehicleDate, p.ReceivedDate));
		}
		return new SampleDto(entity.Id, entity.Date, entity.Name, entity.Hash, entity.LC_FC, points);
	}

	private static Sample MapToEntity(SampleDto dto)
	{
		var points = new List<SamplePoint>(dto.Sample.Count);
		foreach (var p in dto.Sample)
		{
			points.Add(new SamplePoint { Data = p.Data, VehicleDate = p.VehicleDate, ReceivedDate = p.ReceivedDate });
		}
		return new Sample
		{
			Id = dto.Id,
			Date = dto.Date,
			Name = dto.Name,
			Hash = dto.Hash,
			LC_FC = dto.LC_FC,
			SamplePoints = points
		};
	}
}
