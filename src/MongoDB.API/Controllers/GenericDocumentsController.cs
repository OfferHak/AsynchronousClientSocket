using Microsoft.AspNetCore.Mvc;
using MongoDB.Application.Interfaces;
using System.Text.Json;

namespace MongoDB.API.Controllers;

[ApiController]
[Route("api/collections/{collectionName}/documents")]
[Produces("application/json")]
public class GenericDocumentsController : ControllerBase
{
    private readonly IGenericDocumentService _documentService;
    private readonly ILogger<GenericDocumentsController> _logger;

    public GenericDocumentsController(
        IGenericDocumentService documentService,
        ILogger<GenericDocumentsController> logger)
    {
        _documentService = documentService;
        _logger = logger;
    }

    /// <summary>
    /// Get all documents from a collection with pagination
    /// </summary>
    /// <param name="collectionName">Collection name</param>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <returns>Paginated list of documents</returns>
    [HttpGet]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse<MongoDB.Application.Common.PaginatedResult<JsonDocument>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllDocuments(
        string collectionName,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(collectionName))
        {
            return BadRequest("Collection name is required.");
        }

        if (page < 1 || pageSize < 1 || pageSize > 100)
        {
            return BadRequest("Invalid pagination parameters. Page must be >= 1 and pageSize must be between 1 and 100.");
        }

        var result = await _documentService.GetAllDocumentsAsync(collectionName, page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Get a document by ID from a collection
    /// </summary>
    /// <param name="collectionName">Collection name</param>
    /// <param name="id">Document ID</param>
    /// <returns>Document details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse<JsonDocument>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDocumentById(string collectionName, string id)
    {
        if (string.IsNullOrWhiteSpace(collectionName))
        {
            return BadRequest("Collection name is required.");
        }

        var result = await _documentService.GetDocumentByIdAsync(id, collectionName);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Create a new document in a collection
    /// </summary>
    /// <param name="collectionName">Collection name</param>
    /// <param name="document">Document data as JSON</param>
    /// <returns>Created document</returns>
    [HttpPost]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse<JsonDocument>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateDocument(string collectionName, [FromBody] JsonDocument document)
    {
        if (string.IsNullOrWhiteSpace(collectionName))
        {
            return BadRequest("Collection name is required.");
        }

        if (document == null)
        {
            return BadRequest("Document data is required.");
        }

        var result = await _documentService.CreateDocumentAsync(document, collectionName);
        return result.Success ? CreatedAtAction(nameof(GetDocumentById), new { collectionName, id = result.Data?.RootElement.GetProperty("Id").GetString() }, result) : BadRequest(result);
    }

    /// <summary>
    /// Update an existing document in a collection
    /// </summary>
    /// <param name="collectionName">Collection name</param>
    /// <param name="id">Document ID</param>
    /// <param name="document">Updated document data as JSON</param>
    /// <returns>Updated document</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse<JsonDocument>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDocument(string collectionName, string id, [FromBody] JsonDocument document)
    {
        if (string.IsNullOrWhiteSpace(collectionName))
        {
            return BadRequest("Collection name is required.");
        }

        if (document == null)
        {
            return BadRequest("Document data is required.");
        }

        var result = await _documentService.UpdateDocumentAsync(id, document, collectionName);
        
        if (!result.Success)
        {
            return result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
        }
        
        return Ok(result);
    }

    /// <summary>
    /// Delete a document from a collection
    /// </summary>
    /// <param name="collectionName">Collection name</param>
    /// <param name="id">Document ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteDocument(string collectionName, string id)
    {
        if (string.IsNullOrWhiteSpace(collectionName))
        {
            return BadRequest("Collection name is required.");
        }

        var result = await _documentService.DeleteDocumentAsync(id, collectionName);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Get the count of documents in a collection
    /// </summary>
    /// <param name="collectionName">Collection name</param>
    /// <returns>Document count</returns>
    [HttpGet("count")]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse<long>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDocumentCount(string collectionName)
    {
        if (string.IsNullOrWhiteSpace(collectionName))
        {
            return BadRequest("Collection name is required.");
        }

        var result = await _documentService.GetDocumentCountAsync(collectionName);
        return Ok(result);
    }
}