using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MyApp.Infrastructure;

namespace MyApp.API.Controllers;

[ApiController]
[Route("api/health")]
public sealed class HealthController : ControllerBase
{
	private readonly MongoContext _context;

	public HealthController(MongoContext context)
	{
		_context = context;
	}

	[HttpGet("mongo")]
	public async Task<IActionResult> Mongo(CancellationToken ct)
	{
		try
		{
			await _context.Client.ListDatabaseNamesAsync(cancellationToken: ct);
			return Ok(new { status = "ok" });
		}
		catch (Exception ex)
		{
			return StatusCode(500, new { status = "error", message = ex.Message });
		}
	}
}

