using Flashcards.CQRS;

namespace Flashcards.Events;

public record CardUpdated(Guid CardId, string Front, string Back) : EventBase;