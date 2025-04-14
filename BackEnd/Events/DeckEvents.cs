using Flashcards.CQRS;

namespace Flashcards.Events;

public record DeckCreated(Guid DeckId, string Name, string Description) : EventBase;

public record DeckUpdated(Guid DeckId, string Name, string Description) : EventBase;

public record DeckDeleted(Guid DeckId) : EventBase;