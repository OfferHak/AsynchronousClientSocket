using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MongoDB.Application;
using MongoDB.Infrastructure;
using MongoDB.API.Hubs;
using Serilog;
using MongoDB.Domain;

var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDb"));

// Logging
builder.Host.UseSerilog((context, services, configuration) =>
{
	configuration
		.Enrich.FromLogContext()
		.Enrich.WithEnvironmentName()
		.WriteTo.Console();
});

// MVC + JSON
builder.Services.AddControllers().AddNewtonsoftJson();

// SignalR
builder.Services.AddSignalR().AddJsonProtocol();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "MongoDB API", Version = "v1" });
});

// DI registrations
builder.Services.AddSingleton<IMongoDbContext, MongoDbContext>();
builder.Services.AddSingleton(typeof(IRepository<>), typeof(GenericRepository<>));
builder.Services.AddSingleton<ISampleRepository, SampleRepository>();
builder.Services.AddSingleton<IClientRegistry, InMemoryClientRegistry>();
builder.Services.AddSingleton<ISampleAddedNotifier, ApiSignalRSampleAddedNotifier>();
builder.Services.AddHostedService<MongoChangeStreamBackgroundService>();
builder.Services.AddScoped<ISampleService, SampleService>();
builder.Services.AddScoped<IGenericCollectionService, GenericCollectionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.MapHub<SampleHub>("/hubs/samples");

app.Run();
