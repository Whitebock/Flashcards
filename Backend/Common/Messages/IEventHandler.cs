namespace Flashcards.Common.Messages;

public interface IAsyncEventHandler<TEvent> : IEventHandler where TEvent : IEvent
{
    Task HandleAsync(TEvent @event);
}

public interface IEventHandler<TEvent> : IEventHandler where TEvent : IEvent
{
    void Handle(TEvent @event);
}

public interface IEventHandler { }