using Flashcards.CQRS;
using System.Collections.Concurrent;

namespace Flashcards;

public class InMemoryEventStore : IEventStore
{
    private readonly List<IEvent> _store = [];

    public Task SaveAsync(IEvent @event)
    {
        if (@event == null) throw new ArgumentNullException(nameof(@event), "Event cannot be null.");

        _store.Add(@event);
        return Task.CompletedTask;
    }

    public async IAsyncEnumerable<IEvent> GetEventsAsync()
    {
        foreach (var @event in _store)
        {
            yield return @event;
            await Task.Yield();
        }
    }
}