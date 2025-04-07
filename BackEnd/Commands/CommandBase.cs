using Flashcards.CQRS;

namespace Flashcards.Commands;

public abstract record CommandBase : ICommand
{
    public Guid MessageId { get; private set; }
    public DateTime Timestamp { get; private set; }

    protected CommandBase()
    {
        MessageId = Guid.NewGuid();
        Timestamp = DateTime.UtcNow;
    }
}