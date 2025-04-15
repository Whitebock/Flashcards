using Flashcards.CQRS;

namespace Flashcards.Commands;

public record DeleteCardCommand(Guid CardId) : CommandBase;