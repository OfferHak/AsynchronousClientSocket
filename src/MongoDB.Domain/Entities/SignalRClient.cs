using MongoDB.Bson.Serialization.Attributes;

namespace MongoDB.Domain.Entities;

public class SignalRClient : BaseEntity
{
    [BsonElement("connectionId")]
    public string ConnectionId { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("isActive")]
    public bool IsActive { get; set; } = true;

    [BsonElement("lastActivity")]
    public DateTime LastActivity { get; set; } = DateTime.UtcNow;
}