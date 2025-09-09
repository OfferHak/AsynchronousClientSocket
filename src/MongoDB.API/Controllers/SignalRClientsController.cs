using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Application.DTOs;
using MongoDB.Application.Interfaces;

namespace MongoDB.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SignalRClientsController : ControllerBase
{
    private readonly ISignalRClientService _clientService;
    private readonly IValidator<RegisterClientDto> _registerValidator;
    private readonly ILogger<SignalRClientsController> _logger;

    public SignalRClientsController(
        ISignalRClientService clientService,
        IValidator<RegisterClientDto> registerValidator,
        ILogger<SignalRClientsController> logger)
    {
        _clientService = clientService;
        _registerValidator = registerValidator;
        _logger = logger;
    }

    /// <summary>
    /// Get all active SignalR clients
    /// </summary>
    /// <returns>List of active clients</returns>
    [HttpGet("active")]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse<List<SignalRClientDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActiveClients()
    {
        var result = await _clientService.GetActiveClientsAsync();
        return Ok(result);
    }

    /// <summary>
    /// Get a client by name
    /// </summary>
    /// <param name="name">Client name</param>
    /// <returns>Client details</returns>
    [HttpGet("by-name/{name}")]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse<SignalRClientDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetClientByName(string name)
    {
        var result = await _clientService.GetClientByNameAsync(name);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Register a SignalR client (for testing purposes - normally done via SignalR hub)
    /// </summary>
    /// <param name="registerClientDto">Client registration data</param>
    /// <returns>Registered client details</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse<SignalRClientDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterClient([FromBody] RegisterClientDto registerClientDto)
    {
        var validationResult = await _registerValidator.ValidateAsync(registerClientDto);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return BadRequest(MongoDB.Application.Common.ApiResponse<object>.ErrorResponse("Validation failed", errors));
        }

        // Generate a dummy connection ID for testing
        var connectionId = Guid.NewGuid().ToString();
        var result = await _clientService.RegisterClientAsync(connectionId, registerClientDto.Name);
        
        return result.Success ? CreatedAtAction(nameof(GetClientByName), new { name = registerClientDto.Name }, result) : BadRequest(result);
    }

    /// <summary>
    /// Unregister a SignalR client by connection ID
    /// </summary>
    /// <param name="connectionId">Connection ID</param>
    /// <returns>Success status</returns>
    [HttpDelete("unregister/{connectionId}")]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnregisterClient(string connectionId)
    {
        var result = await _clientService.UnregisterClientAsync(connectionId);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Update last activity for a client
    /// </summary>
    /// <param name="connectionId">Connection ID</param>
    /// <returns>Success status</returns>
    [HttpPut("activity/{connectionId}")]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(MongoDB.Application.Common.ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateLastActivity(string connectionId)
    {
        var result = await _clientService.UpdateLastActivityAsync(connectionId);
        return result.Success ? Ok(result) : NotFound(result);
    }
}