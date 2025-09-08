using Microsoft.AspNetCore.Mvc;
using MongoDB.Application;

namespace MongoDB.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ClientsController : ControllerBase
{
	private readonly IClientRegistry _registry;

	public ClientsController(IClientRegistry registry)
	{
		_registry = registry;
	}

	[HttpGet]
	public async Task<IActionResult> Get(CancellationToken cancellationToken)
		=> Ok(await _registry.GetRegisteredClientsAsync(cancellationToken));
}

