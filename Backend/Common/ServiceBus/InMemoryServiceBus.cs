using System.Collections.Concurrent;
using Flashcards.Common.Messages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Flashcards.Common.ServiceBus;

public class InMemoryServiceBus(ILogger<InMemoryServiceBus> logger) : BackgroundService, IServiceBus
{
    private readonly Dictionary<string, ConcurrentQueue<IMessage>> _queues = [];

    public event Func<IMessage, Task> RecievedAsync = delegate { return Task.CompletedTask; };

    public Task PublishAsync(string queue, IMessage message)
    {
        if (!_queues.ContainsKey(queue))
        {
            logger.LogDebug("Creating queue {Queue}", queue);
            _queues[queue] = new ConcurrentQueue<IMessage>();
        }
        logger.LogDebug("Publishing {MessageType} on {Queue} queue", message.GetType().Name, queue);
        _queues[queue].Enqueue(message);
        return Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested) {
            for (int i = 0; i < _queues.Count; i++)
            {
                var queue = _queues.ElementAt(i);
                if (!queue.Value.IsEmpty && queue.Value.TryDequeue(out var message))
                {
                    logger.LogDebug("Revieved {MessageType} on {Queue} queue", message.GetType().Name, queue.Key);
                    await RecievedAsync.Invoke(message);
                }
            }
            
            await Task.Delay(100, stoppingToken);
        }
    }
}