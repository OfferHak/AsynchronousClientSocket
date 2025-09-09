using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Domain.Interfaces;
using MongoDB.Infrastructure.Configuration;
using MongoDB.Infrastructure.Data;

namespace MongoDB.Infrastructure.Services;

public class ChangeStreamService : BackgroundService, IChangeStreamService
{
    private readonly MongoDbContext _context;
    private readonly ILogger<ChangeStreamService> _logger;
    private readonly MongoDbSettings _settings;
    private readonly INotificationService _notificationService;
    private IChangeStreamCursor<ChangeStreamDocument<BsonDocument>>? _changeStreamCursor;
    private bool _isRunning;

    public ChangeStreamService(
        MongoDbContext context,
        ILogger<ChangeStreamService> logger,
        IOptions<MongoDbSettings> settings,
        INotificationService notificationService)
    {
        _context = context;
        _logger = logger;
        _settings = settings.Value;
        _notificationService = notificationService;
    }

    public bool IsRunning => _isRunning;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_settings.EnableChangeStreams)
        {
            _logger.LogInformation("Change streams are disabled in configuration");
            return;
        }

        await StartChangeStreamAsync(stoppingToken);
    }

    public async Task StartChangeStreamAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting MongoDB Change Stream monitoring");
            _isRunning = true;

            // Watch for changes in the samples collection
            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<BsonDocument>>()
                .Match(change => change.OperationType == ChangeStreamOperationType.Insert ||
                               change.OperationType == ChangeStreamOperationType.Update ||
                               change.OperationType == ChangeStreamOperationType.Replace);

            var options = new ChangeStreamOptions
            {
                FullDocument = ChangeStreamFullDocumentOption.UpdateLookup
            };

            // Watch the entire database for changes
            _changeStreamCursor = _context.Database.Watch(pipeline, options, cancellationToken);

            while (await _changeStreamCursor.MoveNextAsync(cancellationToken))
            {
                foreach (var change in _changeStreamCursor.Current)
                {
                    await ProcessChangeAsync(change);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Change stream monitoring was cancelled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in change stream monitoring");
        }
        finally
        {
            _isRunning = false;
        }
    }

    public async Task StopChangeStreamAsync()
    {
        try
        {
            _logger.LogInformation("Stopping MongoDB Change Stream monitoring");
            
            if (_changeStreamCursor != null)
            {
                _changeStreamCursor.Dispose();
                _changeStreamCursor = null;
            }
            
            _isRunning = false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping change stream");
        }
        
        await Task.CompletedTask;
    }

    private async Task ProcessChangeAsync(ChangeStreamDocument<BsonDocument> change)
    {
        try
        {
            _logger.LogDebug("Processing change stream event: {OperationType} on {Collection}", 
                change.OperationType, change.CollectionNamespace?.CollectionName);

            // Only process changes to the samples collection
            if (change.CollectionNamespace?.CollectionName != "samples")
            {
                return;
            }

            switch (change.OperationType)
            {
                case ChangeStreamOperationType.Insert:
                    await HandleInsertAsync(change);
                    break;
                case ChangeStreamOperationType.Update:
                case ChangeStreamOperationType.Replace:
                    await HandleUpdateAsync(change);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing change stream event");
        }
    }

    private async Task HandleInsertAsync(ChangeStreamDocument<BsonDocument> change)
    {
        if (change.FullDocument != null)
        {
            var document = change.FullDocument.ToJson();
            _logger.LogInformation("New sample inserted: {DocumentId}", change.DocumentKey["_id"]);
            
            // Notify all connected SignalR clients about the new sample
            await _notificationService.NotifyAllClientsAsync("NewSample", document);
        }
    }

    private async Task HandleUpdateAsync(ChangeStreamDocument<BsonDocument> change)
    {
        if (change.FullDocument != null)
        {
            var document = change.FullDocument.ToJson();
            _logger.LogInformation("Sample updated: {DocumentId}", change.DocumentKey["_id"]);
            
            // Notify all connected SignalR clients about the updated sample
            await _notificationService.NotifyAllClientsAsync("SampleUpdated", document);
        }
    }

    public override void Dispose()
    {
        _changeStreamCursor?.Dispose();
        base.Dispose();
    }
}