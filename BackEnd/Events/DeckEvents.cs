namespace Flashcards.Events;

public record DeckCreated(Guid Id, string Name) : EventBase;

public record DeckUpdated(string Name) : EventBase;

public record DeckDeleted() : EventBase;