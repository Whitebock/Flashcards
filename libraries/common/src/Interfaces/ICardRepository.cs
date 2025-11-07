using Flashcards.Common.Infrastructure.Consumers;

namespace Flashcards.Common.Interfaces;

public interface ICardRepository
{
    IEnumerable<CardProjection.CardDto> GetCardsForDeck(Guid deckId);
    Guid GetDeckForCard(Guid cardId);
}