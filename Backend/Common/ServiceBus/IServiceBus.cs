using Flashcards.Common.Messages;
using Microsoft.Extensions.Hosting;

namespace Flashcards.Common.ServiceBus;

public interface IServiceBus : IHostedService
{
    Task PublishAsync(string queue, IMessage message);
    event Func<IMessage, Task> RecievedAsync;
}