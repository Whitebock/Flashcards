using Flashcards.CQRS;

namespace Flashcards.Commands;

public record DeleteDeckCommand(Guid DeckId) : CommandBase;