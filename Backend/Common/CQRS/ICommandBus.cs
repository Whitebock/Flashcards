namespace Flashcards.CQRS;

public interface ICommandBus
{
    Task SendAsync(params ICommand[] commands);
}