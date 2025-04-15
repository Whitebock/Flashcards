using Flashcards.CQRS;

namespace Flashcards.Events;

public record DeckUpdated(Guid DeckId, string Name, string Description) : EventBase;