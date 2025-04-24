namespace Flashcards.Common.Messages.Commands;

public record UpdateDeckCommand(Guid DeckId, string Name, string Description) : CommandBase;