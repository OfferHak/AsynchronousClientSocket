using Microsoft.AspNetCore.SignalR;
using MyApp.Application;

namespace MyApp.API.Hubs;

public interface INotificationsClient
{
	Task SampleAdded(SampleItemDto sample);
}

public sealed class NotificationsHub : Hub<INotificationsClient>
{
	private readonly INotificationSubscriptionService _subscriptions;

	public NotificationsHub(INotificationSubscriptionService subscriptions)
	{
		_subscriptions = subscriptions;
	}

	public async Task RegisterByName(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new HubException("name is required");
		}
		await _subscriptions.SubscribeAsync(Context.ConnectionId, name, Context.ConnectionAborted);
	}

	public async Task UnregisterByName(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new HubException("name is required");
		}
		await _subscriptions.UnsubscribeAsync(Context.ConnectionId, name, Context.ConnectionAborted);
	}

	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		await _subscriptions.RemoveAllForConnectionAsync(Context.ConnectionId, Context.ConnectionAborted);
		await base.OnDisconnectedAsync(exception);
	}
}

