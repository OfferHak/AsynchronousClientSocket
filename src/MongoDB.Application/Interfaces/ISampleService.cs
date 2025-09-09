using MongoDB.Application.Common;
using MongoDB.Application.DTOs;

namespace MongoDB.Application.Interfaces;

public interface ISampleService
{
    Task<ApiResponse<PaginatedResult<SampleDto>>> GetAllSamplesAsync(int page = 1, int pageSize = 10);
    Task<ApiResponse<SampleDto>> GetSampleByIdAsync(string id);
    Task<ApiResponse<List<SampleDto>>> GetSamplesByNameAsync(string name);
    Task<ApiResponse<List<SampleDto>>> GetSamplesByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<ApiResponse<SampleDto>> GetLatestSampleAsync();
    Task<ApiResponse<SampleDto>> CreateSampleAsync(CreateSampleDto createSampleDto);
    Task<ApiResponse<SampleDto>> UpdateSampleAsync(UpdateSampleDto updateSampleDto);
    Task<ApiResponse> DeleteSampleAsync(string id);
}