namespace Flashcards.Common.Messages;

public interface IEventHandler<TEvent> : IEventHandler where TEvent : IEvent
{
    Task HandleAsync(TEvent @event);
}

public interface IEventHandler {}