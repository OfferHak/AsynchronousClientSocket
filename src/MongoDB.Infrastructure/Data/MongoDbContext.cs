using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Infrastructure.Configuration;

namespace MongoDB.Infrastructure.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;
    private readonly MongoClient _client;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var mongoSettings = settings.Value;
        
        var clientSettings = MongoClientSettings.FromConnectionString(mongoSettings.ConnectionString);
        clientSettings.ConnectTimeout = TimeSpan.FromSeconds(mongoSettings.ConnectionTimeoutSeconds);
        clientSettings.SocketTimeout = TimeSpan.FromSeconds(mongoSettings.SocketTimeoutSeconds);
        
        _client = new MongoClient(clientSettings);
        _database = _client.GetDatabase(mongoSettings.DatabaseName);
    }

    public IMongoDatabase Database => _database;
    public MongoClient Client => _client;

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _database.GetCollection<T>(collectionName);
    }
}