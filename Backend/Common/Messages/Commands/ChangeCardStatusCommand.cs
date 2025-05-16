namespace Flashcards.Common.Messages.Commands;

public record ChangeCardStatusCommand(Guid CardId, CardStatus Status) : CommandBase;