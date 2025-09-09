using MongoDB.Domain.Entities;

namespace MongoDB.Domain.Interfaces;

public interface ISignalRClientRepository : IGenericRepository<SignalRClient>
{
    Task<SignalRClient?> GetByConnectionIdAsync(string connectionId);
    Task<SignalRClient?> GetByNameAsync(string name);
    Task<IEnumerable<SignalRClient>> GetActiveClientsAsync();
    Task<bool> DeactivateClientAsync(string connectionId);
    Task<bool> UpdateLastActivityAsync(string connectionId);
}