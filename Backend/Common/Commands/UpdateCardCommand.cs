using Flashcards.CQRS;

namespace Flashcards.Commands;

public record UpdateCardCommand(Guid CardId, string Front, string Back) : CommandBase;