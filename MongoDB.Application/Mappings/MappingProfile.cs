using AutoMapper;
using MongoDB.Application.DTOs;
using MongoDB.Domain.Entities;

namespace MongoDB.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<SampleDocument, SampleDto>();
        CreateMap<SampleDto, SampleDocument>();
        CreateMap<CreateSampleDto, SampleDocument>();
        CreateMap<UpdateSampleDto, SampleDocument>();
        CreateMap<SampleData, SampleDataDto>();
        CreateMap<SampleDataDto, SampleData>();
    }
}