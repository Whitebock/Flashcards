using Flashcards.CQRS;

namespace Flashcards.CQRS;

public abstract record MessageBase : IMessage
{
    public Guid MessageId { get; } = Guid.NewGuid();
    public DateTime Timestamp { get; } = DateTime.UtcNow;
}

public abstract record CommandBase : MessageBase, ICommand;
public abstract record EventBase : MessageBase, IEvent;