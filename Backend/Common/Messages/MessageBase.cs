namespace Flashcards.Common.Messages;

public abstract record MessageBase : IMessage
{
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    public required Guid Creator { get; set; }
}

public abstract record CommandBase : MessageBase, ICommand;
public abstract record EventBase : MessageBase, IEvent;