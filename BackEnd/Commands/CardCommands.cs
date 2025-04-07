namespace Flashcards.Commands;

public record CreateCardCommand(Guid DeckId, string Front, string Back) : CommandBase;

public record UpdateCardCommand(Guid DeckId, Guid CardId, string Front, string Back) : CommandBase;

public record DeleteCardCommand(Guid DeckId, Guid CardId) : CommandBase;