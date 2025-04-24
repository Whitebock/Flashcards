namespace Flashcards.Common.Messages.Events;

public record CardCreated(Guid CardId, Guid DeckId, string Front, string Back) : EventBase;