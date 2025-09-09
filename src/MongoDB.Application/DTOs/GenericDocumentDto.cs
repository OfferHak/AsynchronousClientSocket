using System.Text.Json;

namespace MongoDB.Application.DTOs;

public class GenericDocumentDto
{
    public string Id { get; set; } = string.Empty;
    public JsonDocument Data { get; set; } = JsonDocument.Parse("{}");
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateGenericDocumentDto
{
    public JsonDocument Data { get; set; } = JsonDocument.Parse("{}");
}

public class UpdateGenericDocumentDto
{
    public string Id { get; set; } = string.Empty;
    public JsonDocument Data { get; set; } = JsonDocument.Parse("{}");
}