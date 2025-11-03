using Flashcards.Common.Infrastructure.Consumers;

namespace Flashcards.Common.Interfaces;

public interface IDeckRepository
{
    DecksProjection.DeckDto? GetDeck(Guid deckId);
    IEnumerable<DecksProjection.DeckDto> GetAllDecks();
}