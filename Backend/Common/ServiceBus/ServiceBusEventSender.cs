using Flashcards.Common.EventStore;
using Flashcards.Common.Messages;

namespace Flashcards.Common.ServiceBus;

public class ServiceBusEventSender(IServiceBus serviceBus, IEventStore eventStore) : IEventSender
{
    const string QUEUE = "events";
    public async Task PublishAsync(IEvent @event)
    {
        await serviceBus.PublishAsync(QUEUE, @event);
        await eventStore.SaveAsync(@event);
    }
}