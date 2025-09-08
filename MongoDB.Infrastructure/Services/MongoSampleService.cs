using MongoDB.Domain.Entities;
using MongoDB.Domain.Interfaces;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace MongoDB.Infrastructure.Services;

public class MongoSampleService : ISampleService
{
    private readonly IRepository<SampleDocument> _repository;

    public MongoSampleService(IRepository<SampleDocument> repository)
    {
        _repository = repository;
    }

    public async Task<SampleDocument> CreateSampleAsync(SampleDocument sample)
    {
        return await _repository.CreateAsync(sample);
    }

    public async Task<SampleDocument?> GetSampleByIdAsync(string id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<SampleDocument>> GetAllSamplesAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<IEnumerable<SampleDocument>> GetSamplesByNameAsync(string name)
    {
        Expression<Func<SampleDocument, bool>> predicate = x => x.Name == name;
        return await _repository.FindAsync(predicate);
    }

    public async Task<SampleDocument> UpdateSampleAsync(string id, SampleDocument sample)
    {
        return await _repository.UpdateAsync(id, sample);
    }

    public async Task<bool> DeleteSampleAsync(string id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<long> GetSampleCountAsync()
    {
        return await _repository.CountAsync();
    }

    public async Task<IEnumerable<SampleDocument>> GetPagedSamplesAsync(int page, int pageSize)
    {
        return await _repository.GetPagedAsync(page, pageSize);
    }
}