namespace Flashcards.CQRS;

public interface IMessage
{
    Guid MessageId { get; }
    
    DateTime Timestamp { get; }
}