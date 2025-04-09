using Flashcards.CQRS;

namespace Flashcards.Events;

public record DeckCreated(Guid DeckId, string Name) : EventBase;

public record DeckUpdated(Guid DeckId, string Name) : EventBase;

public record DeckDeleted(Guid DeckId) : EventBase;