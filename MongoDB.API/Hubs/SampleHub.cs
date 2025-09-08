using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Application;

namespace MongoDB.API.Hubs
{
	public interface ISampleClient
	{
		Task SampleAdded(SampleDto sample);
	}

	public sealed class SampleHub : Hub<ISampleClient>
	{
		private readonly IClientRegistry _clientRegistry;

		public SampleHub(IClientRegistry clientRegistry)
		{
			_clientRegistry = clientRegistry;
		}

		public Task Register(string name)
			=> _clientRegistry.RegisterClientAsync(name, Context.ConnectionId);

		public override Task OnDisconnectedAsync(System.Exception? exception)
			=> _clientRegistry.UnregisterClientAsync(Context.ConnectionId);
	}

	public sealed class ApiSignalRSampleAddedNotifier : ISampleAddedNotifier
	{
		private readonly IHubContext<SampleHub, ISampleClient> _hubContext;

		public ApiSignalRSampleAddedNotifier(IHubContext<SampleHub, ISampleClient> hubContext)
		{
			_hubContext = hubContext;
		}

		public Task NotifySampleAddedAsync(SampleDto sample, System.Threading.CancellationToken cancellationToken = default)
			=> _hubContext.Clients.All.SampleAdded(sample);
	}
}