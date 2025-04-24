using Flashcards.Common.Messages;

namespace Flashcards.Common.ServiceBus;

public class InMemoryCommandBus(IEnumerable<ICommandHandler> _commandHandlers) : ICommandBus
{
    public async Task SendAsync(params ICommand[] commands)
    {
        foreach (var command in commands)
        {
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
            var handler = _commandHandlers.FirstOrDefault(handlerType.IsInstanceOfType) 
                ?? throw new InvalidOperationException($"No handler found for command type {command.GetType().Name}");

            var method = handlerType.GetMethod("HandleAsync", [command.GetType()]) 
                ?? throw new InvalidOperationException($"No Handle method found for command type {command.GetType().Name}");
            
            var task = method.Invoke(handler, [command]) as Task 
                ?? throw new InvalidOperationException($"HandleAsync did not return a Task for command type {command.GetType().Name}");
            
            await task;
        }
    }
}
