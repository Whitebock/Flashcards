using Flashcards.CQRS;

namespace Flashcards.Events;

public record DeckCreated(Guid DeckId, string Name, string Description) : EventBase;