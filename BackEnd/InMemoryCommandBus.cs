using Flashcards.CQRS;

namespace Flashcards;

public class InMemoryCommandBus : ICommandBus
{
    private readonly IEnumerable<ICommandHandler> _commandHandlers;
    private readonly IEventBus _eventBus;

    public InMemoryCommandBus(IEnumerable<ICommandHandler> commandHandlers, IEventBus eventBus)
    {
        _commandHandlers = commandHandlers;
        _eventBus = eventBus;
    }

    public void Send(ICommand command)
    {
        var handlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
        var handler = _commandHandlers.FirstOrDefault(handlerType.IsInstanceOfType) ?? 
            throw new InvalidOperationException($"No handler found for command type {command.GetType().Name}");

        var method = handlerType.GetMethod("Handle", [command.GetType()]) ?? 
            throw new InvalidOperationException($"No Handle method found for command type {command.GetType().Name}");

        method.Invoke(handler, [command]);
    }

    public void Send(IEnumerable<ICommand> commands)
    {
        foreach (var command in commands)
        {
            Send(command);
        }
    }
}
