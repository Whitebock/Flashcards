using Flashcards.Common.Messages;

namespace Flashcards.Common.ServiceBus;

public interface ICommandBus
{
    Task SendAsync(params ICommand[] commands);
}