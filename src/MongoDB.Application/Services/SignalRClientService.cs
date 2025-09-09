using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDB.Application.Common;
using MongoDB.Application.DTOs;
using MongoDB.Application.Interfaces;
using MongoDB.Domain.Entities;
using MongoDB.Domain.Interfaces;

namespace MongoDB.Application.Services;

public class SignalRClientService : ISignalRClientService
{
    private readonly ISignalRClientRepository _clientRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<SignalRClientService> _logger;
    private const string CollectionName = "signalr_clients";

    public SignalRClientService(
        ISignalRClientRepository clientRepository,
        IMapper mapper,
        ILogger<SignalRClientService> logger)
    {
        _clientRepository = clientRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<SignalRClientDto>> RegisterClientAsync(string connectionId, string name)
    {
        try
        {
            // Check if client with this name already exists
            var existingClient = await _clientRepository.GetByNameAsync(name);
            if (existingClient != null)
            {
                // Update existing client with new connection ID
                existingClient.ConnectionId = connectionId;
                existingClient.IsActive = true;
                existingClient.LastActivity = DateTime.UtcNow;
                existingClient.UpdatedAt = DateTime.UtcNow;

                var updatedClient = await _clientRepository.UpdateAsync(existingClient, CollectionName);
                var updatedClientDto = _mapper.Map<SignalRClientDto>(updatedClient);
                return ApiResponse<SignalRClientDto>.SuccessResponse(updatedClientDto, "Client reconnected successfully");
            }

            // Create new client
            var newClient = new SignalRClient
            {
                ConnectionId = connectionId,
                Name = name,
                IsActive = true,
                LastActivity = DateTime.UtcNow
            };

            var createdClient = await _clientRepository.CreateAsync(newClient, CollectionName);
            var clientDto = _mapper.Map<SignalRClientDto>(createdClient);

            return ApiResponse<SignalRClientDto>.SuccessResponse(clientDto, "Client registered successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering client {Name} with connection {ConnectionId}", name, connectionId);
            return ApiResponse<SignalRClientDto>.ErrorResponse("Failed to register client");
        }
    }

    public async Task<ApiResponse> UnregisterClientAsync(string connectionId)
    {
        try
        {
            var deactivated = await _clientRepository.DeactivateClientAsync(connectionId);
            if (!deactivated)
            {
                return ApiResponse.ErrorResponse("Client not found");
            }

            return ApiResponse.SuccessResponse("Client unregistered successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unregistering client with connection {ConnectionId}", connectionId);
            return ApiResponse.ErrorResponse("Failed to unregister client");
        }
    }

    public async Task<ApiResponse<List<SignalRClientDto>>> GetActiveClientsAsync()
    {
        try
        {
            var activeClients = await _clientRepository.GetActiveClientsAsync();
            var clientDtos = _mapper.Map<List<SignalRClientDto>>(activeClients);
            return ApiResponse<List<SignalRClientDto>>.SuccessResponse(clientDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active clients");
            return ApiResponse<List<SignalRClientDto>>.ErrorResponse("Failed to retrieve active clients");
        }
    }

    public async Task<ApiResponse<SignalRClientDto>> GetClientByNameAsync(string name)
    {
        try
        {
            var client = await _clientRepository.GetByNameAsync(name);
            if (client == null)
            {
                return ApiResponse<SignalRClientDto>.ErrorResponse("Client not found");
            }

            var clientDto = _mapper.Map<SignalRClientDto>(client);
            return ApiResponse<SignalRClientDto>.SuccessResponse(clientDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving client by name {Name}", name);
            return ApiResponse<SignalRClientDto>.ErrorResponse("Failed to retrieve client");
        }
    }

    public async Task<ApiResponse> UpdateLastActivityAsync(string connectionId)
    {
        try
        {
            var updated = await _clientRepository.UpdateLastActivityAsync(connectionId);
            if (!updated)
            {
                return ApiResponse.ErrorResponse("Client not found");
            }

            return ApiResponse.SuccessResponse("Client activity updated");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating last activity for connection {ConnectionId}", connectionId);
            return ApiResponse.ErrorResponse("Failed to update client activity");
        }
    }
}