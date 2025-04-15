using Flashcards.CQRS;
namespace Flashcards;

public class InMemoryEventBus : IEventBus
{
    private readonly IEnumerable<IEventHandler> _eventHandlers;
    private readonly IEventStore _eventStore;

    public InMemoryEventBus(IEnumerable<IEventHandler> eventHandlers, IEventStore eventStore)
    {
        _eventHandlers = eventHandlers;
        _eventStore = eventStore;
    }

    public Task Publish(IEvent @event)
    {
        _eventStore.Save(@event);
        var handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
        var handlers = _eventHandlers.Where(handlerType.IsInstanceOfType);
        foreach (var handler in handlers)
        {
            var method = handlerType.GetMethod("Handle", [@event.GetType()]) ??
                throw new InvalidOperationException($"No Handle method found for event type {@event.GetType().Name}");

            method.Invoke(handler, [@event]);
        }
        
        return Task.CompletedTask;
    }
}