using Flashcards.CQRS;

namespace Flashcards.Commands;

public record CreateDeckCommand(string Name) : CommandBase {
    public Guid DeckId { get; init; } = Guid.NewGuid();
};

public record UpdateDeckCommand(Guid DeckId, string Name) : CommandBase;

public record DeleteDeckCommand(Guid DeckId) : CommandBase;