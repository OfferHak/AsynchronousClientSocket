using Microsoft.AspNetCore.SignalR;
using MyApp.API.Hubs;
using MyApp.Application;
using MyApp.Infrastructure.Subscription;

namespace MyApp.API.Services;

public sealed class SignalRChangeNotificationDispatcher : IChangeNotificationDispatcher
{
	private readonly IHubContext<NotificationsHub, INotificationsClient> _hubContext;
	private readonly InMemoryNotificationSubscriptionService _subscriptions;

	public SignalRChangeNotificationDispatcher(IHubContext<NotificationsHub, INotificationsClient> hubContext, InMemoryNotificationSubscriptionService subscriptions)
	{
		_hubContext = hubContext;
		_subscriptions = subscriptions;
	}

	public async Task NotifySampleAddedAsync(string name, SampleItemDto sample, CancellationToken cancellationToken)
	{
		var connections = _subscriptions.GetConnectionsForName(name);
		if (connections.Count == 0) return;
		foreach (var connectionId in connections)
		{
			await _hubContext.Clients.Client(connectionId).SampleAdded(sample);
		}
	}
}

