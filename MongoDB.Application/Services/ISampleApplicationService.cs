using MongoDB.Application.DTOs;

namespace MongoDB.Application.Services;

public interface ISampleApplicationService
{
    Task<SampleDto> CreateSampleAsync(CreateSampleDto createSampleDto);
    Task<SampleDto?> GetSampleByIdAsync(string id);
    Task<IEnumerable<SampleDto>> GetAllSamplesAsync();
    Task<IEnumerable<SampleDto>> GetSamplesByNameAsync(string name);
    Task<SampleDto> UpdateSampleAsync(string id, UpdateSampleDto updateSampleDto);
    Task<bool> DeleteSampleAsync(string id);
    Task<long> GetSampleCountAsync();
    Task<PagedResultDto<SampleDto>> GetPagedSamplesAsync(int page, int pageSize);
    Task<IEnumerable<SampleDto>> GetSamplesByDateRangeAsync(DateTime startDate, DateTime endDate);
}