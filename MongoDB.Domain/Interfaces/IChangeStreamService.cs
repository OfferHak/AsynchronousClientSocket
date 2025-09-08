using MongoDB.Domain.Entities;

namespace MongoDB.Domain.Interfaces;

public interface IChangeStreamService
{
    Task StartWatchingAsync();
    Task StopWatchingAsync();
    event EventHandler<SampleDocument>? SampleAdded;
    event EventHandler<SampleDocument>? SampleUpdated;
    event EventHandler<string>? SampleDeleted;
}