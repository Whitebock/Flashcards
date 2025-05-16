namespace Flashcards.Common.Messages.Commands;

public record AddTagCommand(Guid DeckId, string Tag) : CommandBase;