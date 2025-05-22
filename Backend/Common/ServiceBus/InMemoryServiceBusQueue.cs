using System.Collections.Concurrent;
using Flashcards.Common.Messages;

namespace Flashcards.Common.ServiceBus;

public class InMemoryServiceBusQueue() : IServiceBusQueue
{
    private readonly ConcurrentQueue<IMessage> _queue = [];

    public event Action<IMessage> MessageReceived = delegate {};

    public Task PublishAsync(IMessage message, CancellationToken cancellationToken = default)
    {
        _queue.Enqueue(message);
        return Task.CompletedTask;
    }

    internal Task ProcessQueueAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested && !_queue.IsEmpty && _queue.TryDequeue(out var message))
        {
            MessageReceived.Invoke(message);
        }
        return Task.CompletedTask;
    }
}