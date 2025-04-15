using Flashcards.CQRS;

namespace Flashcards.Events;

public record DeckDeleted(Guid DeckId) : EventBase;