using Microsoft.AspNetCore.SignalR;
using MongoDB.Domain.Interfaces;

namespace MongoDB.API.Hubs;

public class NotificationHub : Hub
{
    private readonly INotificationService _notificationService;

    public NotificationHub(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task RegisterClient(string clientName)
    {
        await _notificationService.RegisterClientAsync(clientName, Context.ConnectionId);
        await Clients.Caller.SendAsync("RegistrationConfirmed", $"Client '{clientName}' registered successfully");
    }

    public async Task UnregisterClient()
    {
        await _notificationService.UnregisterClientAsync(Context.ConnectionId);
        await Clients.Caller.SendAsync("UnregistrationConfirmed", "Client unregistered successfully");
    }

    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("Connected", "Connected to notification hub");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await _notificationService.UnregisterClientAsync(Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}