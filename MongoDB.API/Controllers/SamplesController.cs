using Microsoft.AspNetCore.Mvc;
using MongoDB.Application;

namespace MongoDB.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SamplesController : ControllerBase
{
	private readonly ISampleService _service;

	public SamplesController(ISampleService service)
	{
		_service = service;
	}

	[HttpGet]
	public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
		=> Ok(await _service.GetAllAsync(cancellationToken));

	[HttpGet("{id}")]
	public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
	{
		var dto = await _service.GetByIdAsync(id, cancellationToken);
		return dto is null ? NotFound() : Ok(dto);
	}

	[HttpPost]
	public async Task<IActionResult> Create([FromBody] SampleDto request, CancellationToken cancellationToken)
	{
		var created = await _service.InsertAsync(request with { Id = string.Empty }, cancellationToken);
		return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update(string id, [FromBody] SampleDto request, CancellationToken cancellationToken)
		=> await _service.UpdateAsync(id, request, cancellationToken) ? NoContent() : NotFound();

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
		=> await _service.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
}

