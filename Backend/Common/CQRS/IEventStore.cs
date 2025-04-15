namespace Flashcards.CQRS;

public interface IEventStore
{
    void Save(IEvent @event);
    IEnumerable<IEvent> GetEvents();
}