namespace Flashcards.Common.Messages.Commands;

public record DeleteDeckCommand(Guid DeckId) : CommandBase;