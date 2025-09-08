namespace MongoDB.Domain.Interfaces;

public interface INotificationService
{
    Task RegisterClientAsync(string clientName, string connectionId);
    Task UnregisterClientAsync(string connectionId);
    Task NotifyNewSampleAsync(string clientName, object sampleData);
    Task NotifyAllClientsAsync(object data);
    Task<IEnumerable<string>> GetRegisteredClientsAsync();
}