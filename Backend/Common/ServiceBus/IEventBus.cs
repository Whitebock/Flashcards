using Flashcards.Common.Messages;

namespace Flashcards.Common.ServiceBus;

public interface IEventBus
{
    Task PublishAsync(IEvent @event);
}