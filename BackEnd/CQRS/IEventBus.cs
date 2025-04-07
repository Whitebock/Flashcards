namespace Flashcards.CQRS;

public interface IEventBus
{
    Task Publish(IEvent @event);
}