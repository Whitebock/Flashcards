namespace Flashcards.Common.Messages;

public interface IMessage
{
    DateTime Timestamp { get; }
    Guid Creator { get; }
}