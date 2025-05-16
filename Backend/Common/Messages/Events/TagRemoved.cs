namespace Flashcards.Common.Messages.Events;

public record TagRemoved(Guid DeckId, string Tag) : EventBase;