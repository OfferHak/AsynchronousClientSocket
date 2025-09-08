using Microsoft.AspNetCore.Mvc;
using MongoDB.Application;
using MongoDB.Bson;

namespace MongoDB.API.Controllers;

[ApiController]
[Route("api/collections/{collectionName}")]
public sealed class GenericCollectionsController : ControllerBase
{
	private readonly IGenericCollectionService _service;

	public GenericCollectionsController(IGenericCollectionService service)
	{
		_service = service;
	}

	[HttpGet]
	public async Task<IActionResult> GetAll([FromRoute] string collectionName, CancellationToken cancellationToken)
		=> Ok(await _service.GetAllAsync(collectionName, cancellationToken));

	[HttpGet("{id}")]
	public async Task<IActionResult> GetById([FromRoute] string collectionName, [FromRoute] string id, CancellationToken cancellationToken)
	{
		var doc = await _service.GetByIdAsync(collectionName, id, cancellationToken);
		return doc is null ? NotFound() : Ok(doc);
	}

	[HttpPost]
	public async Task<IActionResult> Create([FromRoute] string collectionName, [FromBody] BsonDocument document, CancellationToken cancellationToken)
	{
		var created = await _service.InsertAsync(collectionName, document, cancellationToken);
		var id = created.GetValue("_id", null)?.ToString() ?? string.Empty;
		return CreatedAtAction(nameof(GetById), new { collectionName, id }, created);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> Update([FromRoute] string collectionName, [FromRoute] string id, [FromBody] BsonDocument document, CancellationToken cancellationToken)
		=> await _service.UpdateAsync(collectionName, id, document, cancellationToken) ? NoContent() : NotFound();

	[HttpDelete("{id}")]
	public async Task<IActionResult> Delete([FromRoute] string collectionName, [FromRoute] string id, CancellationToken cancellationToken)
		=> await _service.DeleteAsync(collectionName, id, cancellationToken) ? NoContent() : NotFound();
}

