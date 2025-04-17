namespace Flashcards.CQRS;

public interface IEventStore
{
    Task SaveAsync(IEvent @event);
    IAsyncEnumerable<IEvent> GetEventsAsync();
}