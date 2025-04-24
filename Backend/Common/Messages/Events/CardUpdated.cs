namespace Flashcards.Common.Messages.Events;

public record CardUpdated(Guid CardId, string Front, string Back) : EventBase;