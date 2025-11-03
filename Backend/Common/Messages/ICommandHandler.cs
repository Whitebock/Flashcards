using KafkaFlow;

namespace Flashcards.Common.Messages;

public interface ICommandHandler<in TCommand> : IMessageHandler<TCommand> where TCommand : ICommand
{
    
}
