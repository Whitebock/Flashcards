using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Flashcards.Common.ServiceBus;

public class InMemoryServiceBus(ILogger<InMemoryServiceBus> logger) : BackgroundService, IServiceBus
{
    private Dictionary<string, InMemoryServiceBusQueue> _queues = [];

    public Task<IServiceBusQueue> GetQueueAsync(string name, CancellationToken cancellationToken = default)
    {
        if (_queues.TryGetValue(name, out var queue))
        {
            return Task.FromResult((IServiceBusQueue) queue);
        }
        var createdQueue = new InMemoryServiceBusQueue();
        _queues.Add(name, createdQueue);
        logger.LogDebug("Queue created: {Queue}", name);
        return Task.FromResult((IServiceBusQueue)createdQueue);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested) {
            await Task.WhenAll(_queues.Select(queue => queue.Value.ProcessQueueAsync(stoppingToken)));
            await Task.Delay(100, stoppingToken);
        }
    }
}