using Microsoft.AspNetCore.Mvc;
using MongoDB.Application.DTOs;
using MongoDB.Application.Services;
using MongoDB.API.Hubs;

namespace MongoDB.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SamplesController : ControllerBase
{
    private readonly ISampleApplicationService _sampleService;
    private readonly INotificationService _notificationService;
    private readonly ILogger<SamplesController> _logger;

    public SamplesController(
        ISampleApplicationService sampleService,
        INotificationService notificationService,
        ILogger<SamplesController> logger)
    {
        _sampleService = sampleService;
        _notificationService = notificationService;
        _logger = logger;
    }

    /// <summary>
    /// Get all samples
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SampleDto>>> GetAllSamples()
    {
        try
        {
            var samples = await _sampleService.GetAllSamplesAsync();
            return Ok(samples);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all samples");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get sample by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<SampleDto>> GetSampleById(string id)
    {
        try
        {
            var sample = await _sampleService.GetSampleByIdAsync(id);
            if (sample == null)
                return NotFound($"Sample with ID '{id}' not found");

            return Ok(sample);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sample with ID: {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get samples by name
    /// </summary>
    [HttpGet("by-name/{name}")]
    public async Task<ActionResult<IEnumerable<SampleDto>>> GetSamplesByName(string name)
    {
        try
        {
            var samples = await _sampleService.GetSamplesByNameAsync(name);
            return Ok(samples);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving samples by name: {Name}", name);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get samples by date range
    /// </summary>
    [HttpGet("by-date-range")]
    public async Task<ActionResult<IEnumerable<SampleDto>>> GetSamplesByDateRange(
        [FromQuery] DateTime startDate, 
        [FromQuery] DateTime endDate)
    {
        try
        {
            var samples = await _sampleService.GetSamplesByDateRangeAsync(startDate, endDate);
            return Ok(samples);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving samples by date range: {StartDate} - {EndDate}", startDate, endDate);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get paged samples
    /// </summary>
    [HttpGet("paged")]
    public async Task<ActionResult<PagedResultDto<SampleDto>>> GetPagedSamples(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10)
    {
        try
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var result = await _sampleService.GetPagedSamplesAsync(page, pageSize);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving paged samples: Page {Page}, PageSize {PageSize}", page, pageSize);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Get sample count
    /// </summary>
    [HttpGet("count")]
    public async Task<ActionResult<long>> GetSampleCount()
    {
        try
        {
            var count = await _sampleService.GetSampleCountAsync();
            return Ok(new { Count = count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sample count");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Create a new sample
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SampleDto>> CreateSample([FromBody] CreateSampleDto createSampleDto)
    {
        try
        {
            var sample = await _sampleService.CreateSampleAsync(createSampleDto);
            return CreatedAtAction(nameof(GetSampleById), new { id = sample.Id }, sample);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sample");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Update an existing sample
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<SampleDto>> UpdateSample(string id, [FromBody] UpdateSampleDto updateSampleDto)
    {
        try
        {
            var sample = await _sampleService.UpdateSampleAsync(id, updateSampleDto);
            return Ok(sample);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating sample with ID: {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Delete a sample
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSample(string id)
    {
        try
        {
            var result = await _sampleService.DeleteSampleAsync(id);
            if (!result)
                return NotFound($"Sample with ID '{id}' not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting sample with ID: {Id}", id);
            return StatusCode(500, "Internal server error");
        }
    }
}