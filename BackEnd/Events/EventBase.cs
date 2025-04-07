using Flashcards.CQRS;

namespace Flashcards.Events;

public abstract record EventBase : IEvent
{
    public Guid MessageId { get; private set; }
    public DateTime Timestamp { get; private set; }

    protected EventBase()
    {
        MessageId = Guid.NewGuid();
        Timestamp = DateTime.UtcNow;
    }
}