using Flashcards.Common.Messages;

namespace Flashcards.Common.ServiceBus;

public interface ICommandSender
{
    Task SendAsync(params ICommand[] commands);
}