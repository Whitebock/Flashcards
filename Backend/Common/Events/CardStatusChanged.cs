using Flashcards.CQRS;

namespace Flashcards.Events;

public record CardStatusChanged(Guid CardId, CardStatus Status) : EventBase;