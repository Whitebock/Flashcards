namespace Flashcards.Common.Messages.Commands;

public record CreateCardCommand(Guid DeckId, string Front, string Back) : CommandBase {
    public Guid CardId { get; init; } = Guid.NewGuid();
};