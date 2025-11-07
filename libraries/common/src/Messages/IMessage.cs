namespace Flashcards.Common.Messages;

public interface IMessage
{
    Guid Creator { get; }
}