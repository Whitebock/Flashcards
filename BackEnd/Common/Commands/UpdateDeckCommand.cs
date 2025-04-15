using Flashcards.CQRS;

namespace Flashcards.Commands;

public record UpdateDeckCommand(Guid DeckId, string Name, string Description) : CommandBase;