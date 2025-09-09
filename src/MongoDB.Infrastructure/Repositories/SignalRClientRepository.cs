using MongoDB.Driver;
using MongoDB.Domain.Entities;
using MongoDB.Domain.Interfaces;
using MongoDB.Infrastructure.Data;

namespace MongoDB.Infrastructure.Repositories;

public class SignalRClientRepository : GenericRepository<SignalRClient>, ISignalRClientRepository
{
    private const string CollectionName = "signalr_clients";

    public SignalRClientRepository(MongoDbContext context) : base(context)
    {
    }

    public async Task<SignalRClient?> GetByConnectionIdAsync(string connectionId)
    {
        var collection = _context.GetCollection<SignalRClient>(CollectionName);
        return await collection.Find(x => x.ConnectionId == connectionId).FirstOrDefaultAsync();
    }

    public async Task<SignalRClient?> GetByNameAsync(string name)
    {
        var collection = _context.GetCollection<SignalRClient>(CollectionName);
        return await collection.Find(x => x.Name == name).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<SignalRClient>> GetActiveClientsAsync()
    {
        var collection = _context.GetCollection<SignalRClient>(CollectionName);
        return await collection.Find(x => x.IsActive)
            .SortByDescending(x => x.LastActivity)
            .ToListAsync();
    }

    public async Task<bool> DeactivateClientAsync(string connectionId)
    {
        var collection = _context.GetCollection<SignalRClient>(CollectionName);
        var update = Builders<SignalRClient>.Update
            .Set(x => x.IsActive, false)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        var result = await collection.UpdateOneAsync(x => x.ConnectionId == connectionId, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> UpdateLastActivityAsync(string connectionId)
    {
        var collection = _context.GetCollection<SignalRClient>(CollectionName);
        var update = Builders<SignalRClient>.Update
            .Set(x => x.LastActivity, DateTime.UtcNow)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        var result = await collection.UpdateOneAsync(x => x.ConnectionId == connectionId, update);
        return result.ModifiedCount > 0;
    }
}