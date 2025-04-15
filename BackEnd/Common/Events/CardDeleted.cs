using Flashcards.CQRS;

namespace Flashcards.Events;

public record CardDeleted(Guid CardId) : EventBase;