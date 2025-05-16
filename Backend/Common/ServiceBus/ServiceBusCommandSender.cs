using Flashcards.Common.Messages;

namespace Flashcards.Common.ServiceBus;

public class ServiceBusCommandSender(IServiceBus serviceBus) : ICommandSender
{
    const string QUEUE = "commands";
    public async Task SendAsync(params ICommand[] commands)
    {
        foreach (var command in commands)
        {
            await serviceBus.PublishAsync(QUEUE, command);
        }
    }
}