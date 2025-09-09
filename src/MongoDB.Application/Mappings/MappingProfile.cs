using AutoMapper;
using MongoDB.Application.DTOs;
using MongoDB.Domain.Entities;

namespace MongoDB.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Sample mappings
        CreateMap<Sample, SampleDto>()
            .ForMember(dest => dest.Sample, opt => opt.MapFrom(src => src.SampleData));
        CreateMap<CreateSampleDto, Sample>()
            .ForMember(dest => dest.SampleData, opt => opt.MapFrom(src => src.Sample))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        CreateMap<UpdateSampleDto, Sample>()
            .ForMember(dest => dest.SampleData, opt => opt.MapFrom(src => src.Sample))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        // Sample data mappings
        CreateMap<SampleData, SampleDataDto>();
        CreateMap<CreateSampleDataDto, SampleData>();
        CreateMap<UpdateSampleDataDto, SampleData>();

        // SignalR client mappings
        CreateMap<SignalRClient, SignalRClientDto>();
        CreateMap<RegisterClientDto, SignalRClient>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ConnectionId, opt => opt.Ignore())
            .ForMember(dest => dest.IsActive, opt => opt.Ignore())
            .ForMember(dest => dest.LastActivity, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
}