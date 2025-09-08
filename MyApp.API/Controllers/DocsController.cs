using Microsoft.AspNetCore.Mvc;
using MyApp.Application;

namespace MyApp.API.Controllers;

[ApiController]
[Route("api/docs")]
public sealed class DocsController : ControllerBase
{
	private readonly IMyDocumentQueryService _queries;

	public DocsController(IMyDocumentQueryService queries)
	{
		_queries = queries;
	}

	[HttpGet("latest-by-name/{name}")]
	public async Task<ActionResult<MyDocumentDto>> GetLatestByName([FromRoute] string name, CancellationToken ct)
	{
		if (string.IsNullOrWhiteSpace(name)) return BadRequest("name is required");
		var result = await _queries.GetLatestDocumentByNameAsync(name, ct);
		return result is null ? NotFound() : Ok(result);
	}

	[HttpGet("latest-sample/{name}")]
	public async Task<ActionResult<SampleItemDto>> GetLatestSample([FromRoute] string name, CancellationToken ct)
	{
		if (string.IsNullOrWhiteSpace(name)) return BadRequest("name is required");
		var result = await _queries.GetLatestSampleForNameAsync(name, ct);
		return result is null ? NotFound() : Ok(result);
	}
}

