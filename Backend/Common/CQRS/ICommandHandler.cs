namespace Flashcards.CQRS;

public interface ICommandHandler<TCommand> : ICommandHandler where TCommand : ICommand
{
    Task HandleAsync(TCommand command);
}

public interface ICommandHandler {}