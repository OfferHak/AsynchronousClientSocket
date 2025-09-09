using MongoDB.Application.Common;
using MongoDB.Application.DTOs;

namespace MongoDB.Application.Interfaces;

public interface ISignalRClientService
{
    Task<ApiResponse<SignalRClientDto>> RegisterClientAsync(string connectionId, string name);
    Task<ApiResponse> UnregisterClientAsync(string connectionId);
    Task<ApiResponse<List<SignalRClientDto>>> GetActiveClientsAsync();
    Task<ApiResponse<SignalRClientDto>> GetClientByNameAsync(string name);
    Task<ApiResponse> UpdateLastActivityAsync(string connectionId);
}