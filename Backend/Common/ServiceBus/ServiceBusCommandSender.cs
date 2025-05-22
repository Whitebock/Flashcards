using Flashcards.Common.Messages;

namespace Flashcards.Common.ServiceBus;

public class ServiceBusCommandSender(IServiceBus serviceBus) : ICommandSender
{
    const string QUEUE = "commands";
    public async Task SendAsync(params ICommand[] commands)
    {
        var queue = await serviceBus.GetQueueAsync(QUEUE);
        foreach (var command in commands)
        {
            await queue.PublishAsync(command);
        }
    }
}