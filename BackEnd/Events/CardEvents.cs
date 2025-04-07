namespace Flashcards.Events;

public record CardCreated(Guid DeckId, string Front, string Back) : EventBase;

public record CardUpdated(Guid Deck, string Front, string Back) : EventBase;

public record CardDeleted(Guid Deck) : EventBase;