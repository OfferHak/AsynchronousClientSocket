using Microsoft.AspNetCore.Mvc;
using MongoDB.Application.DTOs;
using MongoDB.Domain.Interfaces;

namespace MongoDB.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
        INotificationService notificationService,
        ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    /// <summary>
    /// Get all registered clients
    /// </summary>
    [HttpGet("clients")]
    public async Task<ActionResult<IEnumerable<string>>> GetRegisteredClients()
    {
        try
        {
            var clients = await _notificationService.GetRegisteredClientsAsync();
            return Ok(clients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving registered clients");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Send notification to all clients
    /// </summary>
    [HttpPost("broadcast")]
    public async Task<ActionResult> BroadcastNotification([FromBody] object data)
    {
        try
        {
            await _notificationService.NotifyAllClientsAsync(data);
            return Ok(new { Message = "Notification sent to all clients" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error broadcasting notification");
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Send notification to specific client
    /// </summary>
    [HttpPost("notify-client")]
    public async Task<ActionResult> NotifyClient([FromBody] ClientNotificationDto notification)
    {
        try
        {
            await _notificationService.NotifyNewSampleAsync(notification.ClientName, notification.Data);
            return Ok(new { Message = $"Notification sent to client '{notification.ClientName}'" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending notification to client: {ClientName}", notification.ClientName);
            return StatusCode(500, "Internal server error");
        }
    }
}

public class ClientNotificationDto
{
    public string ClientName { get; set; } = string.Empty;
    public object Data { get; set; } = new();
}