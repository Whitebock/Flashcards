namespace Flashcards.CQRS;

public interface ICommandBus
{
    void Send(ICommand command);
    void Send(IEnumerable<ICommand> commands);
}