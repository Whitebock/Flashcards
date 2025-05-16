namespace Flashcards.Common.Messages.Commands;

public record RemoveTagCommand(Guid DeckId, string Tag) : CommandBase;