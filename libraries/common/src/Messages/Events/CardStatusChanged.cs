namespace Flashcards.Common.Messages.Events;

public record CardStatusChanged(Guid CardId, CardStatus Status) : EventBase;