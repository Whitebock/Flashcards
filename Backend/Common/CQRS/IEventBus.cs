namespace Flashcards.CQRS;

public interface IEventBus
{
    Task PublishAsync(IEvent @event);
}