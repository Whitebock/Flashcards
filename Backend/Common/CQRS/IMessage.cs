namespace Flashcards.CQRS;

public interface IMessage
{
    DateTime Timestamp { get; }
    Guid Creator { get; }
}