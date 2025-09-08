using Microsoft.AspNetCore.SignalR;
using Microsoft.OpenApi.Models;
using MyApp.API.Hubs;
using MyApp.Application;
using MyApp.Infrastructure;
using MyApp.Infrastructure.Subscription;
using MyApp.API.Services;
using MyApp.API.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "MyApp API",
		Version = "v1"
	});
});

// Options
builder.Services.Configure<MongoOptions>(builder.Configuration.GetSection("Mongo"));

// Infrastructure services
builder.Services.AddSingleton<MongoContext>();
builder.Services.AddSingleton<IMyDocumentRepository, MyDocumentRepository>();
builder.Services.AddSingleton<IChangeNotificationDispatcher, SignalRChangeNotificationDispatcher>();
builder.Services.AddSingleton<IChangeStreamService, ChangeStreamService>();
builder.Services.AddSingleton<INotificationSubscriptionService, InMemoryNotificationSubscriptionService>();
builder.Services.AddHostedService<ChangeStreamHostedService>();

// Application services
builder.Services.AddScoped<IMyDocumentQueryService, MyDocumentQueryService>();

// SignalR
builder.Services.AddSignalR().AddJsonProtocol();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();
app.MapHub<NotificationsHub>("/hub/notifications");

app.Run();
