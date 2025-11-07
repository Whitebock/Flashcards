using Flashcards.Common.Infrastructure.Consumers;

namespace Flashcards.Common.Interfaces;

public interface IDeckUserStatsRepository
{
    DeckUserStatsProjection.DeckStatDto? GetDeckStats(Guid userId, Guid deckId);
}