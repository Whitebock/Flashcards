using Flashcards.Common.Messages;

namespace Flashcards.Common.ServiceBus;

public interface IEventSender
{
    Task PublishAsync(IEvent @event);
}