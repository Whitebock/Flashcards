namespace Flashcards.Common.Messages.Commands;

public record CreateDeckCommand(string Name, string Description) : CommandBase {
    public Guid DeckId { get; init; } = Guid.NewGuid();
};