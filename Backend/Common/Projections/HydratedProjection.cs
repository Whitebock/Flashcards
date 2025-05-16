using Flashcards.Common.EventStore;
using Flashcards.Common.Messages;
using Microsoft.Extensions.Logging;

namespace Flashcards.Common.Projections;

public class HydratedProjection<T>(T projection, IEventStore eventStore, ILogger<HydratedProjection<T>> logger) : IProjection<T> where T : class
{
    private int _increment = 0;
    public async Task<T> GetAsync()
    {
        if (_increment == 0)
        {
            await foreach (var @event in eventStore.GetEventsAsync())
            {
                await ApplyEvent(@event);
                _increment++;
            }
            logger.LogDebug("Hydrated {ProjectionName} using {EventCount} events", projection.GetType().Name, _increment);
        }
        
        return projection;
    }

    private async Task ApplyEvent(IEvent @event)
    {
        var handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());
        var handlerMethod = handlerType.GetMethod("HandleAsync", [@event.GetType()])
            ?? throw new InvalidOperationException($"No Handle method found for event type {@event.GetType().Name}");

        var supportsEvent = projection?.GetType().GetInterfaces().Where(i => i.Equals(handlerType)).FirstOrDefault() != null;
        if (!supportsEvent) return;

        var task = handlerMethod?.Invoke(projection, [@event]) as Task
            ?? throw new InvalidOperationException($"HandleAsync did not return a Task for event type {@event.GetType().Name}");
        await task;
    }
}