using Flashcards.CQRS;

namespace Flashcards.Events;

public record CardCreated(Guid CardId, Guid DeckId, string Front, string Back) : EventBase;

public record CardUpdated(Guid CardId, string Front, string Back) : EventBase;

public record CardDeleted(Guid CardId) : EventBase;

public record CardStatusChanged(Guid CardId, CardStatus Status): EventBase;