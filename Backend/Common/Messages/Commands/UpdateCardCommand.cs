namespace Flashcards.Common.Messages.Commands;

public record UpdateCardCommand(Guid CardId, string Front, string Back) : CommandBase;