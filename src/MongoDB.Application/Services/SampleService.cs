using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDB.Application.Common;
using MongoDB.Application.DTOs;
using MongoDB.Application.Interfaces;
using MongoDB.Domain.Entities;
using MongoDB.Domain.Interfaces;

namespace MongoDB.Application.Services;

public class SampleService : ISampleService
{
    private readonly ISampleRepository _sampleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<SampleService> _logger;
    private readonly INotificationService _notificationService;
    private const string CollectionName = "samples";

    public SampleService(
        ISampleRepository sampleRepository,
        IMapper mapper,
        ILogger<SampleService> logger,
        INotificationService notificationService)
    {
        _sampleRepository = sampleRepository;
        _mapper = mapper;
        _logger = logger;
        _notificationService = notificationService;
    }

    public async Task<ApiResponse<PaginatedResult<SampleDto>>> GetAllSamplesAsync(int page = 1, int pageSize = 10)
    {
        try
        {
            var samples = await _sampleRepository.GetAllAsync(CollectionName);
            var totalCount = await _sampleRepository.CountAsync(CollectionName);

            var paginatedSamples = samples
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var sampleDtos = _mapper.Map<List<SampleDto>>(paginatedSamples);
            var result = new PaginatedResult<SampleDto>(sampleDtos, (int)totalCount, page, pageSize);

            return ApiResponse<PaginatedResult<SampleDto>>.SuccessResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving samples");
            return ApiResponse<PaginatedResult<SampleDto>>.ErrorResponse("Failed to retrieve samples");
        }
    }

    public async Task<ApiResponse<SampleDto>> GetSampleByIdAsync(string id)
    {
        try
        {
            var sample = await _sampleRepository.GetByIdAsync(id, CollectionName);
            if (sample == null)
            {
                return ApiResponse<SampleDto>.ErrorResponse("Sample not found");
            }

            var sampleDto = _mapper.Map<SampleDto>(sample);
            return ApiResponse<SampleDto>.SuccessResponse(sampleDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving sample with id {Id}", id);
            return ApiResponse<SampleDto>.ErrorResponse("Failed to retrieve sample");
        }
    }

    public async Task<ApiResponse<List<SampleDto>>> GetSamplesByNameAsync(string name)
    {
        try
        {
            var samples = await _sampleRepository.GetSamplesByNameAsync(name);
            var sampleDtos = _mapper.Map<List<SampleDto>>(samples);
            return ApiResponse<List<SampleDto>>.SuccessResponse(sampleDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving samples by name {Name}", name);
            return ApiResponse<List<SampleDto>>.ErrorResponse("Failed to retrieve samples by name");
        }
    }

    public async Task<ApiResponse<List<SampleDto>>> GetSamplesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var samples = await _sampleRepository.GetSamplesByDateRangeAsync(startDate, endDate);
            var sampleDtos = _mapper.Map<List<SampleDto>>(samples);
            return ApiResponse<List<SampleDto>>.SuccessResponse(sampleDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving samples by date range {StartDate} - {EndDate}", startDate, endDate);
            return ApiResponse<List<SampleDto>>.ErrorResponse("Failed to retrieve samples by date range");
        }
    }

    public async Task<ApiResponse<SampleDto>> GetLatestSampleAsync()
    {
        try
        {
            var sample = await _sampleRepository.GetLatestSampleAsync();
            if (sample == null)
            {
                return ApiResponse<SampleDto>.ErrorResponse("No samples found");
            }

            var sampleDto = _mapper.Map<SampleDto>(sample);
            return ApiResponse<SampleDto>.SuccessResponse(sampleDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving latest sample");
            return ApiResponse<SampleDto>.ErrorResponse("Failed to retrieve latest sample");
        }
    }

    public async Task<ApiResponse<SampleDto>> CreateSampleAsync(CreateSampleDto createSampleDto)
    {
        try
        {
            var sample = _mapper.Map<Sample>(createSampleDto);
            var createdSample = await _sampleRepository.CreateAsync(sample, CollectionName);
            var sampleDto = _mapper.Map<SampleDto>(createdSample);

            // Notify clients about new sample
            await _notificationService.NotifyNewSampleAsync(sampleDto);

            return ApiResponse<SampleDto>.SuccessResponse(sampleDto, "Sample created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sample");
            return ApiResponse<SampleDto>.ErrorResponse("Failed to create sample");
        }
    }

    public async Task<ApiResponse<SampleDto>> UpdateSampleAsync(UpdateSampleDto updateSampleDto)
    {
        try
        {
            var existingSample = await _sampleRepository.GetByIdAsync(updateSampleDto.Id, CollectionName);
            if (existingSample == null)
            {
                return ApiResponse<SampleDto>.ErrorResponse("Sample not found");
            }

            _mapper.Map(updateSampleDto, existingSample);
            existingSample.UpdatedAt = DateTime.UtcNow;

            var updatedSample = await _sampleRepository.UpdateAsync(existingSample, CollectionName);
            var sampleDto = _mapper.Map<SampleDto>(updatedSample);

            return ApiResponse<SampleDto>.SuccessResponse(sampleDto, "Sample updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating sample with id {Id}", updateSampleDto.Id);
            return ApiResponse<SampleDto>.ErrorResponse("Failed to update sample");
        }
    }

    public async Task<ApiResponse> DeleteSampleAsync(string id)
    {
        try
        {
            var deleted = await _sampleRepository.DeleteAsync(id, CollectionName);
            if (!deleted)
            {
                return ApiResponse.ErrorResponse("Sample not found");
            }

            return ApiResponse.SuccessResponse("Sample deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting sample with id {Id}", id);
            return ApiResponse.ErrorResponse("Failed to delete sample");
        }
    }
}