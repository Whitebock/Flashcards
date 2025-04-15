using Flashcards.CQRS;

namespace Flashcards.Commands;

public record ChangeCardStatus(Guid CardId, CardStatus Status) : CommandBase;