using Flashcards.Common.Messages;

namespace Flashcards.Common.EventStore;

public interface IEventStore
{
    Task SaveAsync(IEvent @event);
    IAsyncEnumerable<IEvent> GetEventsAsync();
}