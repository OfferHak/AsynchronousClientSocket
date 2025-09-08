using AutoMapper;
using MongoDB.Application.DTOs;
using MongoDB.Application.Validators;
using MongoDB.Domain.Interfaces;
using FluentValidation;

namespace MongoDB.Application.Services;

public class SampleApplicationService : ISampleApplicationService
{
    private readonly ISampleService _sampleService;
    private readonly IMapper _mapper;
    private readonly CreateSampleDtoValidator _createValidator;
    private readonly UpdateSampleDtoValidator _updateValidator;

    public SampleApplicationService(
        ISampleService sampleService,
        IMapper mapper,
        CreateSampleDtoValidator createValidator,
        UpdateSampleDtoValidator updateValidator)
    {
        _sampleService = sampleService;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<SampleDto> CreateSampleAsync(CreateSampleDto createSampleDto)
    {
        await _createValidator.ValidateAndThrowAsync(createSampleDto);
        
        var sample = _mapper.Map<SampleDocument>(createSampleDto);
        var createdSample = await _sampleService.CreateSampleAsync(sample);
        
        return _mapper.Map<SampleDto>(createdSample);
    }

    public async Task<SampleDto?> GetSampleByIdAsync(string id)
    {
        var sample = await _sampleService.GetSampleByIdAsync(id);
        return sample != null ? _mapper.Map<SampleDto>(sample) : null;
    }

    public async Task<IEnumerable<SampleDto>> GetAllSamplesAsync()
    {
        var samples = await _sampleService.GetAllSamplesAsync();
        return _mapper.Map<IEnumerable<SampleDto>>(samples);
    }

    public async Task<IEnumerable<SampleDto>> GetSamplesByNameAsync(string name)
    {
        var samples = await _sampleService.GetSamplesByNameAsync(name);
        return _mapper.Map<IEnumerable<SampleDto>>(samples);
    }

    public async Task<SampleDto> UpdateSampleAsync(string id, UpdateSampleDto updateSampleDto)
    {
        await _updateValidator.ValidateAndThrowAsync(updateSampleDto);
        
        var sample = _mapper.Map<SampleDocument>(updateSampleDto);
        var updatedSample = await _sampleService.UpdateSampleAsync(id, sample);
        
        return _mapper.Map<SampleDto>(updatedSample);
    }

    public async Task<bool> DeleteSampleAsync(string id)
    {
        return await _sampleService.DeleteSampleAsync(id);
    }

    public async Task<long> GetSampleCountAsync()
    {
        return await _sampleService.GetSampleCountAsync();
    }

    public async Task<PagedResultDto<SampleDto>> GetPagedSamplesAsync(int page, int pageSize)
    {
        var samples = await _sampleService.GetPagedSamplesAsync(page, pageSize);
        var totalCount = await _sampleService.GetSampleCountAsync();
        
        return new PagedResultDto<SampleDto>
        {
            Data = _mapper.Map<IEnumerable<SampleDto>>(samples),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<IEnumerable<SampleDto>> GetSamplesByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var allSamples = await _sampleService.GetAllSamplesAsync();
        var filteredSamples = allSamples.Where(s => s.Date >= startDate && s.Date <= endDate);
        return _mapper.Map<IEnumerable<SampleDto>>(filteredSamples);
    }
}