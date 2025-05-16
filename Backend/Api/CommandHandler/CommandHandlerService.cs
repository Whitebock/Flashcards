
using Flashcards.Common.Messages;
using Flashcards.Common.ServiceBus;

namespace Flashcards.Api.CommandHandler;

public class CommandHandlerService(IServiceBus serviceBus, IEnumerable<ICommandHandler> commandHandlers) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        serviceBus.RecievedAsync += OnMessageRecieved;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        serviceBus.RecievedAsync -= OnMessageRecieved;
        return Task.CompletedTask;
    }

    private async Task OnMessageRecieved(IMessage message)
    {
        if (message is not ICommand command) return;

        var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
        var handler = commandHandlers.FirstOrDefault(handlerType.IsInstanceOfType) 
            ?? throw new InvalidOperationException($"No handler found for command type {command.GetType().Name}");

        var method = handlerType.GetMethod("HandleAsync", [command.GetType()]) 
            ?? throw new InvalidOperationException($"No Handle method found for command type {command.GetType().Name}");
        
        var task = method.Invoke(handler, [command]) as Task 
            ?? throw new InvalidOperationException($"HandleAsync did not return a Task for command type {command.GetType().Name}");
        
        await task;
    }
}