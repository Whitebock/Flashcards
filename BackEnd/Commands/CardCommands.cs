using Flashcards.CQRS;

namespace Flashcards.Commands;

public record CreateCardCommand(Guid DeckId, string Front, string Back) : CommandBase {
    public Guid CardId { get; init; } = Guid.NewGuid();
};

public record UpdateCardCommand(Guid CardId, string Front, string Back) : CommandBase;

public record DeleteCardCommand(Guid CardId) : CommandBase;

public record ChangeCardStatus(Guid CardId, CardStatus Status): CommandBase;