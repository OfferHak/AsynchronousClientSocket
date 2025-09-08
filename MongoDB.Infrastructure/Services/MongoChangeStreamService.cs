using MongoDB.Driver;
using MongoDB.Domain.Entities;
using MongoDB.Domain.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MongoDB.Infrastructure.Services;

public class MongoChangeStreamService : BackgroundService, IChangeStreamService
{
    private readonly IMongoCollection<SampleDocument> _collection;
    private readonly INotificationService _notificationService;
    private readonly ILogger<MongoChangeStreamService> _logger;
    private IChangeStreamCursor<ChangeStreamDocument<SampleDocument>>? _changeStream;

    public event EventHandler<SampleDocument>? SampleAdded;
    public event EventHandler<SampleDocument>? SampleUpdated;
    public event EventHandler<string>? SampleDeleted;

    public MongoChangeStreamService(
        IMongoDatabase database,
        INotificationService notificationService,
        ILogger<MongoChangeStreamService> logger)
    {
        _collection = database.GetCollection<SampleDocument>("samples");
        _notificationService = notificationService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await StartWatchingAsync();
            
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in change stream service");
        }
    }

    public async Task StartWatchingAsync()
    {
        try
        {
            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<SampleDocument>>()
                .Match(change => change.OperationType == ChangeStreamOperationType.Insert ||
                                change.OperationType == ChangeStreamOperationType.Update ||
                                change.OperationType == ChangeStreamOperationType.Delete);

            _changeStream = await _collection.WatchAsync(pipeline);

            _ = Task.Run(async () =>
            {
                await foreach (var change in _changeStream.ToAsyncEnumerable())
                {
                    await ProcessChangeAsync(change);
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting change stream");
        }
    }

    public async Task StopWatchingAsync()
    {
        _changeStream?.Dispose();
        _changeStream = null;
    }

    private async Task ProcessChangeAsync(ChangeStreamDocument<SampleDocument> change)
    {
        try
        {
            switch (change.OperationType)
            {
                case ChangeStreamOperationType.Insert:
                    if (change.FullDocument != null)
                    {
                        SampleAdded?.Invoke(this, change.FullDocument);
                        await _notificationService.NotifyAllClientsAsync(change.FullDocument);
                        _logger.LogInformation("New sample added: {SampleId}", change.FullDocument.Id);
                    }
                    break;

                case ChangeStreamOperationType.Update:
                    if (change.FullDocument != null)
                    {
                        SampleUpdated?.Invoke(this, change.FullDocument);
                        _logger.LogInformation("Sample updated: {SampleId}", change.FullDocument.Id);
                    }
                    break;

                case ChangeStreamOperationType.Delete:
                    if (change.DocumentKey.TryGetValue("_id", out var id))
                    {
                        SampleDeleted?.Invoke(this, id.ToString()!);
                        _logger.LogInformation("Sample deleted: {SampleId}", id);
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing change stream event");
        }
    }

    public override void Dispose()
    {
        _changeStream?.Dispose();
        base.Dispose();
    }
}