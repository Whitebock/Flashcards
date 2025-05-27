using Flashcards.Common.EventStore;
using Flashcards.Common.Messages;
using Microsoft.Extensions.Logging;

namespace Flashcards.Common.Projections;

public class HydratedProjection<T>(T projection, IEventStore eventStore, ILogger<HydratedProjection<T>> logger) : IProjection<T> where T : class
{
    private int _increment = 0;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    public async Task<T> GetAsync()
    {
        if (_increment == 0)
        {
            await _semaphore.WaitAsync();
            if (_increment > 0)
            {
                _semaphore.Release();
                return projection;
            }
            await foreach (var @event in eventStore.GetEventsAsync())
            {
                await ApplyEvent(@event);
                _increment++;
            }
            logger.LogDebug("Hydrated {ProjectionName} using {EventCount} events", projection.GetType().Name, _increment);
            _semaphore.Release();
        }

        return projection;
    }

    private async Task ApplyEvent(IEvent @event)
    {
        var handlerInterface = projection?.GetType()
            .GetInterfaces()
            .Where(i => i.IsAssignableTo(typeof(IEventHandler)))
            .Where(i => i.GenericTypeArguments.Contains(@event.GetType()))
            .FirstOrDefault();

        if (handlerInterface == null) return;

        var handlerMethod = (handlerInterface.GetGenericTypeDefinition() == typeof(IEventHandler<>)
            ? handlerInterface.GetMethod("Handle", [@event.GetType()])
            : handlerInterface.GetMethod("HandleAsync", [@event.GetType()]))
            ?? throw new InvalidOperationException($"No handle method found for event type {@event.GetType().Name} in projection {projection!.GetType().Name}");
        
        var result = handlerMethod.Invoke(projection, [@event]);
        if (result is Task task) await task;
    }
}