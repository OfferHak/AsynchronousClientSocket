using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MongoDB.Domain.Interfaces;

namespace MongoDB.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly ISignalRClientRepository _clientRepository;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IHubContext<NotificationHub> hubContext,
        ISignalRClientRepository clientRepository,
        ILogger<NotificationService> logger)
    {
        _hubContext = hubContext;
        _clientRepository = clientRepository;
        _logger = logger;
    }

    public async Task NotifyNewSampleAsync(object sample)
    {
        try
        {
            await _hubContext.Clients.All.SendAsync("NewSample", sample);
            _logger.LogInformation("Notified all clients about new sample");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying clients about new sample");
        }
    }

    public async Task NotifyClientAsync(string clientName, string message, object? data = null)
    {
        try
        {
            var client = await _clientRepository.GetByNameAsync(clientName);
            if (client != null && client.IsActive)
            {
                await _hubContext.Clients.Client(client.ConnectionId).SendAsync("Notification", message, data);
                _logger.LogInformation("Notified client {ClientName} with message: {Message}", clientName, message);
            }
            else
            {
                _logger.LogWarning("Client {ClientName} not found or inactive", clientName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying client {ClientName}", clientName);
        }
    }

    public async Task NotifyAllClientsAsync(string message, object? data = null)
    {
        try
        {
            await _hubContext.Clients.All.SendAsync("Notification", message, data);
            _logger.LogInformation("Notified all clients with message: {Message}", message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying all clients");
        }
    }

    public async Task RegisterClientAsync(string connectionId, string clientName)
    {
        try
        {
            await _hubContext.Groups.AddToGroupAsync(connectionId, "RegisteredClients");
            _logger.LogInformation("Registered client {ClientName} with connection {ConnectionId}", clientName, connectionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering client {ClientName}", clientName);
        }
    }

    public async Task UnregisterClientAsync(string connectionId)
    {
        try
        {
            await _hubContext.Groups.RemoveFromGroupAsync(connectionId, "RegisteredClients");
            _logger.LogInformation("Unregistered client with connection {ConnectionId}", connectionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unregistering client with connection {ConnectionId}", connectionId);
        }
    }
}

// SignalR Hub
public class NotificationHub : Hub
{
    private readonly ISignalRClientRepository _clientRepository;
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(
        ISignalRClientRepository clientRepository,
        ILogger<NotificationHub> logger)
    {
        _clientRepository = clientRepository;
        _logger = logger;
    }

    public async Task RegisterClient(string clientName)
    {
        try
        {
            var client = await _clientRepository.GetByNameAsync(clientName);
            if (client != null)
            {
                // Update existing client
                client.ConnectionId = Context.ConnectionId;
                client.IsActive = true;
                client.LastActivity = DateTime.UtcNow;
                client.UpdatedAt = DateTime.UtcNow;
                
                await _clientRepository.UpdateAsync(client, "signalr_clients");
            }
            else
            {
                // Create new client
                var newClient = new MongoDB.Domain.Entities.SignalRClient
                {
                    ConnectionId = Context.ConnectionId,
                    Name = clientName,
                    IsActive = true,
                    LastActivity = DateTime.UtcNow
                };
                
                await _clientRepository.CreateAsync(newClient, "signalr_clients");
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, "RegisteredClients");
            await Clients.Caller.SendAsync("ClientRegistered", $"Successfully registered as {clientName}");
            
            _logger.LogInformation("Client {ClientName} registered with connection {ConnectionId}", clientName, Context.ConnectionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering client {ClientName}", clientName);
            await Clients.Caller.SendAsync("Error", "Failed to register client");
        }
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            await _clientRepository.DeactivateClientAsync(Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "RegisteredClients");
            
            _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling client disconnection for {ConnectionId}", Context.ConnectionId);
        }
        
        await base.OnDisconnectedAsync(exception);
    }

    public async Task UpdateActivity()
    {
        try
        {
            await _clientRepository.UpdateLastActivityAsync(Context.ConnectionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating activity for connection {ConnectionId}", Context.ConnectionId);
        }
    }
}