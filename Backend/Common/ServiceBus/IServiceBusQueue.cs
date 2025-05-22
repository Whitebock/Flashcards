using Flashcards.Common.Messages;

namespace Flashcards.Common.ServiceBus;

public interface IServiceBusQueue
{
    Task PublishAsync(IMessage message, CancellationToken cancellationToken = default);
    event Action<IMessage> MessageReceived;
}