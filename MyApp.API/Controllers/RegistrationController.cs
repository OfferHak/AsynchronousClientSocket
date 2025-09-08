using Microsoft.AspNetCore.Mvc;
using MyApp.Application;

namespace MyApp.API.Controllers;

[ApiController]
[Route("api/registration")]
public sealed class RegistrationController : ControllerBase
{
	private readonly INotificationSubscriptionService _subs;

	public RegistrationController(INotificationSubscriptionService subs)
	{
		_subs = subs;
	}

	public sealed record RegisterRequest(string ConnectionId, string Name);

	[HttpPost]
	public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
	{
		if (string.IsNullOrWhiteSpace(request.ConnectionId) || string.IsNullOrWhiteSpace(request.Name))
		{
			return BadRequest("ConnectionId and Name are required");
		}
		await _subs.SubscribeAsync(request.ConnectionId, request.Name, ct);
		return Ok();
	}
}

