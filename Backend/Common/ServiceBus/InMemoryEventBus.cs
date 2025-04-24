using Flashcards.Common.EventStore;
using Flashcards.Common.Messages;

namespace Flashcards.Common.ServiceBus;

public class InMemoryEventBus : IEventBus
{
    private readonly IEnumerable<IEventHandler> _eventHandlers;
    private readonly IEventStore _eventStore;

    public InMemoryEventBus(IEnumerable<IEventHandler> eventHandlers, IEventStore eventStore)
    {
        _eventHandlers = eventHandlers;
        _eventStore = eventStore;
    }

    public async Task PublishAsync(IEvent @event)
    {
        await _eventStore.SaveAsync(@event);
        ApplyToHandlers(@event);
    }

    public void ApplyToHandlers(IEvent @event) {
        var handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
        var handlers = _eventHandlers.Where(handlerType.IsInstanceOfType);
        foreach (var handler in handlers)
        {
            var method = handlerType.GetMethod("Handle", [@event.GetType()]) ??
                throw new InvalidOperationException($"No Handle method found for event type {@event.GetType().Name}");

            method.Invoke(handler, [@event]);
        }
    }
}