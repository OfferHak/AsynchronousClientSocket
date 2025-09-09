namespace MongoDB.Domain.Interfaces;

public interface IChangeStreamService
{
    Task StartChangeStreamAsync(CancellationToken cancellationToken = default);
    Task StopChangeStreamAsync();
    bool IsRunning { get; }
}