using MongoDB.Domain.Entities;

namespace MongoDB.Domain.Interfaces;

public interface ISampleService
{
    Task<SampleDocument> CreateSampleAsync(SampleDocument sample);
    Task<SampleDocument?> GetSampleByIdAsync(string id);
    Task<IEnumerable<SampleDocument>> GetAllSamplesAsync();
    Task<IEnumerable<SampleDocument>> GetSamplesByNameAsync(string name);
    Task<SampleDocument> UpdateSampleAsync(string id, SampleDocument sample);
    Task<bool> DeleteSampleAsync(string id);
    Task<long> GetSampleCountAsync();
    Task<IEnumerable<SampleDocument>> GetPagedSamplesAsync(int page, int pageSize);
}