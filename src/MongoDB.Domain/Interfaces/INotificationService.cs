namespace MongoDB.Domain.Interfaces;

public interface INotificationService
{
    Task NotifyNewSampleAsync(object sample);
    Task NotifyClientAsync(string clientName, string message, object? data = null);
    Task NotifyAllClientsAsync(string message, object? data = null);
    Task RegisterClientAsync(string connectionId, string clientName);
    Task UnregisterClientAsync(string connectionId);
}