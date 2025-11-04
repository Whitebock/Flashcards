namespace Flashcards.Common.Messages;

public abstract record MessageBase : IMessage
{
    public required Guid Creator { get; init; }
}

public abstract record CommandBase : MessageBase, ICommand;
public abstract record EventBase : MessageBase, IEvent;