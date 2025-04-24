namespace Flashcards.Common.Messages.Events;

public record DeckCreated(Guid DeckId, string Name, string Description) : EventBase;