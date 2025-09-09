using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Application.Interfaces;
using MongoDB.Application.Mappings;
using MongoDB.Application.Services;
using System.Reflection;

namespace MongoDB.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(typeof(MappingProfile));

        // FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Services
        services.AddScoped<ISampleService, SampleService>();
        services.AddScoped<IGenericDocumentService, GenericDocumentService>();
        services.AddScoped<ISignalRClientService, SignalRClientService>();

        return services;
    }
}