namespace Flashcards.Common.Messages.Commands;

public record DeleteCardCommand(Guid CardId) : CommandBase;