namespace Flashcards.Common.Messages.Events;

public record DeckUpdated(Guid DeckId, string Name, string Description) : EventBase;