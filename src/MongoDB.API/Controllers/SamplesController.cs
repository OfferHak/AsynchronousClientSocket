using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Application.DTOs;
using MongoDB.Application.Interfaces;

namespace MongoDB.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SamplesController : ControllerBase
{
    private readonly ISampleService _sampleService;
    private readonly IValidator<CreateSampleDto> _createValidator;
    private readonly ILogger<SamplesController> _logger;

    public SamplesController(
        ISampleService sampleService,
        IValidator<CreateSampleDto> createValidator,
        ILogger<SamplesController> logger)
    {
        _sampleService = sampleService;
        _createValidator = createValidator;
        _logger = logger;
    }

    /// <summary>
    /// Get all samples with pagination
    /// </summary>
    /// <param name="page">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 10)</param>
    /// <returns>Paginated list of samples</returns>
    [HttpGet]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse<MongoDB.Application.Common.PaginatedResult<SampleDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllSamples([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
        {
            return BadRequest("Invalid pagination parameters. Page must be >= 1 and pageSize must be between 1 and 100.");
        }

        var result = await _sampleService.GetAllSamplesAsync(page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Get a sample by ID
    /// </summary>
    /// <param name="id">Sample ID</param>
    /// <returns>Sample details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse<SampleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSampleById(string id)
    {
        var result = await _sampleService.GetSampleByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Get samples by name (partial match)
    /// </summary>
    /// <param name="name">Sample name to search for</param>
    /// <returns>List of matching samples</returns>
    [HttpGet("by-name/{name}")]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse<List<SampleDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSamplesByName(string name)
    {
        var result = await _sampleService.GetSamplesByNameAsync(name);
        return Ok(result);
    }

    /// <summary>
    /// Get samples by date range
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>List of samples within date range</returns>
    [HttpGet("by-date-range")]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse<List<SampleDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetSamplesByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        if (startDate > endDate)
        {
            return BadRequest("Start date must be before or equal to end date.");
        }

        var result = await _sampleService.GetSamplesByDateRangeAsync(startDate, endDate);
        return Ok(result);
    }

    /// <summary>
    /// Get the latest sample
    /// </summary>
    /// <returns>Latest sample</returns>
    [HttpGet("latest")]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse<SampleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetLatestSample()
    {
        var result = await _sampleService.GetLatestSampleAsync();
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Create a new sample
    /// </summary>
    /// <param name="createSampleDto">Sample data</param>
    /// <returns>Created sample</returns>
    [HttpPost]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse<SampleDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSample([FromBody] CreateSampleDto createSampleDto)
    {
        var validationResult = await _createValidator.ValidateAsync(createSampleDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(MongoDB.Application.Common.ApiResponse<object>.ErrorResponse("Validation failed", errors));
        }

        var result = await _sampleService.CreateSampleAsync(createSampleDto);
        return result.Success ? CreatedAtAction(nameof(GetSampleById), new { id = result.Data?.Id }, result) : BadRequest(result);
    }

    /// <summary>
    /// Update an existing sample
    /// </summary>
    /// <param name="id">Sample ID</param>
    /// <param name="updateSampleDto">Updated sample data</param>
    /// <returns>Updated sample</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse<SampleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSample(string id, [FromBody] UpdateSampleDto updateSampleDto)
    {
        updateSampleDto.Id = id;
        var result = await _sampleService.UpdateSampleAsync(updateSampleDto);
        
        if (!result.Success)
        {
            return result.Message.Contains("not found") ? NotFound(result) : BadRequest(result);
        }
        
        return Ok(result);
    }

    /// <summary>
    /// Delete a sample
    /// </summary>
    /// <param name="id">Sample ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSample(string id)
    {
        var result = await _sampleService.DeleteSampleAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }
}