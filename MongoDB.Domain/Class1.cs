using System;
using System.Collections.Generic;

namespace MongoDB.Domain;

public sealed class Sample
{
	public string Id { get; init; } = string.Empty;
	public DateTime Date { get; init; }
	public string Name { get; init; } = string.Empty;
	public double Hash { get; init; }
	public int LC_FC { get; init; }
	public IReadOnlyList<SamplePoint> SamplePoints { get; init; } = Array.Empty<SamplePoint>();
}

public sealed class SamplePoint
{
	public float Data { get; init; }
	public DateTime VehicleDate { get; init; }
	public DateTime ReceivedDate { get; init; }
}

public interface IRepository<TDocument>
{
	Task<TDocument?> GetByIdAsync(string collectionName, string id, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<TDocument>> GetAllAsync(string collectionName, CancellationToken cancellationToken = default);
	Task<TDocument> InsertAsync(string collectionName, TDocument document, CancellationToken cancellationToken = default);
	Task<bool> UpdateAsync(string collectionName, string id, TDocument document, CancellationToken cancellationToken = default);
	Task<bool> DeleteAsync(string collectionName, string id, CancellationToken cancellationToken = default);
}

public interface ISampleRepository
{
	Task<Sample?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<Sample>> GetAllAsync(CancellationToken cancellationToken = default);
	Task<Sample> InsertAsync(Sample sample, CancellationToken cancellationToken = default);
	Task<bool> UpdateAsync(string id, Sample sample, CancellationToken cancellationToken = default);
	Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
}
