using Microsoft.AspNetCore.SignalR;
using MongoDB.Domain.Interfaces;

namespace MongoDB.Infrastructure.Services;

public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly Dictionary<string, string> _clientConnections = new();

    public SignalRNotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task RegisterClientAsync(string clientName, string connectionId)
    {
        _clientConnections[connectionId] = clientName;
        await _hubContext.Clients.Client(connectionId).SendAsync("ClientRegistered", clientName);
    }

    public async Task UnregisterClientAsync(string connectionId)
    {
        if (_clientConnections.ContainsKey(connectionId))
        {
            _clientConnections.Remove(connectionId);
        }
    }

    public async Task NotifyNewSampleAsync(string clientName, object sampleData)
    {
        var clientConnections = _clientConnections
            .Where(kvp => kvp.Value == clientName)
            .Select(kvp => kvp.Key)
            .ToList();

        if (clientConnections.Any())
        {
            await _hubContext.Clients.Clients(clientConnections).SendAsync("NewSampleAdded", sampleData);
        }
    }

    public async Task NotifyAllClientsAsync(object data)
    {
        await _hubContext.Clients.All.SendAsync("BroadcastMessage", data);
    }

    public async Task<IEnumerable<string>> GetRegisteredClientsAsync()
    {
        return await Task.FromResult(_clientConnections.Values.Distinct());
    }
}