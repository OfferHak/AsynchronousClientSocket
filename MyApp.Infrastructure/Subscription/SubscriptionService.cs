using System.Collections.Concurrent;
using MyApp.Application;

namespace MyApp.Infrastructure.Subscription;

public sealed class InMemoryNotificationSubscriptionService : INotificationSubscriptionService
{
	private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> _nameToConnections = new();
	private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> _connectionToNames = new();

	public Task SubscribeAsync(string connectionId, string name, CancellationToken cancellationToken)
	{
		var set = _nameToConnections.GetOrAdd(name, _ => new());
		set[connectionId] = 1;
		var back = _connectionToNames.GetOrAdd(connectionId, _ => new());
		back[name] = 1;
		return Task.CompletedTask;
	}

	public Task UnsubscribeAsync(string connectionId, string name, CancellationToken cancellationToken)
	{
		if (_nameToConnections.TryGetValue(name, out var set))
		{
			set.TryRemove(connectionId, out _);
			if (set.IsEmpty) _nameToConnections.TryRemove(name, out _);
		}
		if (_connectionToNames.TryGetValue(connectionId, out var back))
		{
			back.TryRemove(name, out _);
			if (back.IsEmpty) _connectionToNames.TryRemove(connectionId, out _);
		}
		return Task.CompletedTask;
	}

	public Task RemoveAllForConnectionAsync(string connectionId, CancellationToken cancellationToken)
	{
		if (_connectionToNames.TryRemove(connectionId, out var names))
		{
			foreach (var name in names.Keys)
			{
				if (_nameToConnections.TryGetValue(name, out var conns))
				{
					conns.TryRemove(connectionId, out _);
					if (conns.IsEmpty) _nameToConnections.TryRemove(name, out _);
				}
			}
		}
		return Task.CompletedTask;
	}

	public IReadOnlyCollection<string> GetConnectionsForName(string name)
	{
		if (_nameToConnections.TryGetValue(name, out var set))
		{
			return set.Keys.ToArray();
		}
		return Array.Empty<string>();
	}
}

