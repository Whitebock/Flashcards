namespace Flashcards.Common.Messages.Events;

public record CardDeleted(Guid CardId) : EventBase;