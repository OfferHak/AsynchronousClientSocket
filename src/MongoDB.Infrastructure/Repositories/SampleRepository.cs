using MongoDB.Driver;
using MongoDB.Domain.Entities;
using MongoDB.Domain.Interfaces;
using MongoDB.Infrastructure.Data;

namespace MongoDB.Infrastructure.Repositories;

public class SampleRepository : GenericRepository<Sample>, ISampleRepository
{
    private const string CollectionName = "samples";

    public SampleRepository(MongoDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Sample>> GetSamplesByNameAsync(string name)
    {
        var collection = _context.GetCollection<Sample>(CollectionName);
        return await collection.Find(x => x.Name.ToLower().Contains(name.ToLower())).ToListAsync();
    }

    public async Task<IEnumerable<Sample>> GetSamplesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var collection = _context.GetCollection<Sample>(CollectionName);
        return await collection.Find(x => x.Date >= startDate && x.Date <= endDate)
            .SortByDescending(x => x.Date)
            .ToListAsync();
    }

    public async Task<Sample?> GetLatestSampleAsync()
    {
        var collection = _context.GetCollection<Sample>(CollectionName);
        return await collection.Find(_ => true)
            .SortByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync();
    }
}