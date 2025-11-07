namespace Flashcards.Common.Messages.Events;

public record DeckDeleted(Guid DeckId) : EventBase;