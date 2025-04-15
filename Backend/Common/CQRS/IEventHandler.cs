namespace Flashcards.CQRS;

public interface IEventHandler<TEvent> : IEventHandler where TEvent : IEvent
{
    void Handle(TEvent @event);
}

public interface IEventHandler {}