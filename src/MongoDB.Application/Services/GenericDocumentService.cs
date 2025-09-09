using Microsoft.Extensions.Logging;
using MongoDB.Application.Common;
using MongoDB.Application.Interfaces;
using MongoDB.Domain.Entities;
using MongoDB.Domain.Interfaces;
using System.Text.Json;

namespace MongoDB.Application.Services;

public class GenericDocumentService : IGenericDocumentService
{
    private readonly IGenericRepository<BaseEntity> _genericRepository;
    private readonly ILogger<GenericDocumentService> _logger;

    public GenericDocumentService(
        IGenericRepository<BaseEntity> genericRepository,
        ILogger<GenericDocumentService> logger)
    {
        _genericRepository = genericRepository;
        _logger = logger;
    }

    public async Task<ApiResponse<PaginatedResult<JsonDocument>>> GetAllDocumentsAsync(string collectionName, int page = 1, int pageSize = 10)
    {
        try
        {
            var documents = await _genericRepository.GetAllAsync(collectionName);
            var totalCount = await _genericRepository.CountAsync(collectionName);

            var paginatedDocuments = documents
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(doc => JsonSerializer.SerializeToDocument(doc))
                .ToList();

            var result = new PaginatedResult<JsonDocument>(paginatedDocuments, (int)totalCount, page, pageSize);
            return ApiResponse<PaginatedResult<JsonDocument>>.SuccessResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving documents from collection {CollectionName}", collectionName);
            return ApiResponse<PaginatedResult<JsonDocument>>.ErrorResponse("Failed to retrieve documents");
        }
    }

    public async Task<ApiResponse<JsonDocument>> GetDocumentByIdAsync(string id, string collectionName)
    {
        try
        {
            var document = await _genericRepository.GetByIdAsync(id, collectionName);
            if (document == null)
            {
                return ApiResponse<JsonDocument>.ErrorResponse("Document not found");
            }

            var jsonDocument = JsonSerializer.SerializeToDocument(document);
            return ApiResponse<JsonDocument>.SuccessResponse(jsonDocument);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving document with id {Id} from collection {CollectionName}", id, collectionName);
            return ApiResponse<JsonDocument>.ErrorResponse("Failed to retrieve document");
        }
    }

    public async Task<ApiResponse<JsonDocument>> CreateDocumentAsync(JsonDocument document, string collectionName)
    {
        try
        {
            var entity = JsonSerializer.Deserialize<BaseEntity>(document.RootElement.GetRawText()) ?? new GenericDocument();
            var createdDocument = await _genericRepository.CreateAsync(entity, collectionName);
            var jsonDocument = JsonSerializer.SerializeToDocument(createdDocument);

            return ApiResponse<JsonDocument>.SuccessResponse(jsonDocument, "Document created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating document in collection {CollectionName}", collectionName);
            return ApiResponse<JsonDocument>.ErrorResponse("Failed to create document");
        }
    }

    public async Task<ApiResponse<JsonDocument>> UpdateDocumentAsync(string id, JsonDocument document, string collectionName)
    {
        try
        {
            var existingDocument = await _genericRepository.GetByIdAsync(id, collectionName);
            if (existingDocument == null)
            {
                return ApiResponse<JsonDocument>.ErrorResponse("Document not found");
            }

            var updatedEntity = JsonSerializer.Deserialize<BaseEntity>(document.RootElement.GetRawText());
            if (updatedEntity != null)
            {
                updatedEntity.Id = id;
                updatedEntity.UpdatedAt = DateTime.UtcNow;
                updatedEntity.CreatedAt = existingDocument.CreatedAt;

                var updatedDocument = await _genericRepository.UpdateAsync(updatedEntity, collectionName);
                var jsonDocument = JsonSerializer.SerializeToDocument(updatedDocument);

                return ApiResponse<JsonDocument>.SuccessResponse(jsonDocument, "Document updated successfully");
            }

            return ApiResponse<JsonDocument>.ErrorResponse("Invalid document format");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating document with id {Id} in collection {CollectionName}", id, collectionName);
            return ApiResponse<JsonDocument>.ErrorResponse("Failed to update document");
        }
    }

    public async Task<ApiResponse> DeleteDocumentAsync(string id, string collectionName)
    {
        try
        {
            var deleted = await _genericRepository.DeleteAsync(id, collectionName);
            if (!deleted)
            {
                return ApiResponse.ErrorResponse("Document not found");
            }

            return ApiResponse.SuccessResponse("Document deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting document with id {Id} from collection {CollectionName}", id, collectionName);
            return ApiResponse.ErrorResponse("Failed to delete document");
        }
    }

    public async Task<ApiResponse<long>> GetDocumentCountAsync(string collectionName)
    {
        try
        {
            var count = await _genericRepository.CountAsync(collectionName);
            return ApiResponse<long>.SuccessResponse(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting document count from collection {CollectionName}", collectionName);
            return ApiResponse<long>.ErrorResponse("Failed to get document count");
        }
    }
}