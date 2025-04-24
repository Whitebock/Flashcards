namespace Flashcards.Common.Messages.Commands;

public record ChangeCardStatus(Guid CardId, CardStatus Status) : CommandBase;