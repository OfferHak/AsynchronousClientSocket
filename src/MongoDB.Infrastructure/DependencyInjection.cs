using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Domain.Entities;
using MongoDB.Domain.Interfaces;
using MongoDB.Infrastructure.Configuration;
using MongoDB.Infrastructure.Data;
using MongoDB.Infrastructure.Repositories;
using MongoDB.Infrastructure.Services;

namespace MongoDB.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // MongoDB Configuration
        services.Configure<MongoDbSettings>(configuration.GetSection(MongoDbSettings.SectionName));
        
        // MongoDB Context
        services.AddSingleton<MongoDbContext>();
        
        // Repositories
        services.AddScoped<IGenericRepository<BaseEntity>, GenericRepository<BaseEntity>>();
        services.AddScoped<ISampleRepository, SampleRepository>();
        services.AddScoped<ISignalRClientRepository, SignalRClientRepository>();
        
        // Services
        services.AddScoped<INotificationService, NotificationService>();
        services.AddSingleton<IChangeStreamService, ChangeStreamService>();
        services.AddHostedService<ChangeStreamService>();

        return services;
    }
}