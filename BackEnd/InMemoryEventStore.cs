using Flashcards.CQRS;

namespace Flashcards;

public class InMemoryEventStore : IEventStore
{
    private readonly List<IEvent> _store = new();

    public void Save(IEvent @event)
    {
        _store.Add(@event);
    }

    public IEnumerable<IEvent> GetEvents()
    {
        return _store;
    }
}