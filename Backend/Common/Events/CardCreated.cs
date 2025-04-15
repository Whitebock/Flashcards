using Flashcards.CQRS;

namespace Flashcards.Events;

public record CardCreated(Guid CardId, Guid DeckId, string Front, string Back) : EventBase;