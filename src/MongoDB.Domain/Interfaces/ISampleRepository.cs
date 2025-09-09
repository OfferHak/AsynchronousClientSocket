using MongoDB.Domain.Entities;

namespace MongoDB.Domain.Interfaces;

public interface ISampleRepository : IGenericRepository<Sample>
{
    Task<IEnumerable<Sample>> GetSamplesByNameAsync(string name);
    Task<IEnumerable<Sample>> GetSamplesByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<Sample?> GetLatestSampleAsync();
}