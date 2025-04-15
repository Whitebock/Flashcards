using Flashcards.CQRS;

namespace Flashcards.Commands;

public record CreateCardCommand(Guid DeckId, string Front, string Back) : CommandBase {
    public Guid CardId { get; init; } = Guid.NewGuid();
};