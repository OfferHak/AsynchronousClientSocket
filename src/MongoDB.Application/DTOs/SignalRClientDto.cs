namespace MongoDB.Application.DTOs;

public class SignalRClientDto
{
    public string Id { get; set; } = string.Empty;
    public string ConnectionId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime LastActivity { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RegisterClientDto
{
    public string Name { get; set; } = string.Empty;
}