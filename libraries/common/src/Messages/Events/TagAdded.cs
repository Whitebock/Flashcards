namespace Flashcards.Common.Messages.Events;

public record TagAdded(Guid DeckId, string Tag) : EventBase;