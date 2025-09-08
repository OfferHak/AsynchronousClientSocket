using Microsoft.Extensions.Hosting;
using MyApp.Infrastructure;

namespace MyApp.API.Services;

public sealed class ChangeStreamHostedService : IHostedService
{
	private readonly IChangeStreamService _service;
	private CancellationTokenSource? _cts;

	public ChangeStreamHostedService(IChangeStreamService service)
	{
		_service = service;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
		_ = Task.Run(() => _service.StartAsync(_cts.Token), cancellationToken);
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_cts?.Cancel();
		return Task.CompletedTask;
	}
}

