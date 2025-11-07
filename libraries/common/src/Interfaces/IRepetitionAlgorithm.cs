using Flashcards.Common.Infrastructure.Consumers;

namespace Flashcards.Common.Interfaces;

public interface IRepetitionAlgorithm
{
    Task<IEnumerable<CardProjection.CardDto>> GetStudyBatchAsync(Guid userId, Guid deckId);
}