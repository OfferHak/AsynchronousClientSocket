using MongoDB.Application.Common;
using MongoDB.Application.DTOs;
using System.Text.Json;

namespace MongoDB.Application.Interfaces;

public interface IGenericDocumentService
{
    Task<ApiResponse<PaginatedResult<JsonDocument>>> GetAllDocumentsAsync(string collectionName, int page = 1, int pageSize = 10);
    Task<ApiResponse<JsonDocument>> GetDocumentByIdAsync(string id, string collectionName);
    Task<ApiResponse<JsonDocument>> CreateDocumentAsync(JsonDocument document, string collectionName);
    Task<ApiResponse<JsonDocument>> UpdateDocumentAsync(string id, JsonDocument document, string collectionName);
    Task<ApiResponse> DeleteDocumentAsync(string id, string collectionName);
    Task<ApiResponse<long>> GetDocumentCountAsync(string collectionName);
}