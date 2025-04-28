namespace Flashcards.Common.Messages;

public interface IEventHandler<TEvent> : IEventHandler where TEvent : IEvent
{
    // TODO: Make Handlers async.
    void Handle(TEvent @event);
}

public interface IEventHandler {}