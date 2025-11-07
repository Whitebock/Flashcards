using Flashcards.Common.Interfaces;
using Flashcards.Common.Messages;
using Flashcards.Common.Messages.Events;
using KafkaFlow;

namespace Flashcards.Common.Infrastructure.Consumers;
public class SpacedRepetitionProjection(ICardRepository cardRepository) :
    IMessageHandler<CardStatusChanged>, IRepetitionAlgorithm
{
    private readonly FreeSpacedRepetitionScheduler _scheduler = new();

    public async Task<IEnumerable<CardProjection.CardDto>> GetStudyBatchAsync(Guid userId, Guid deckId)
    {
        var dueCardIds = _scheduler.GetCardInfos(userId).Where(kv => kv.Value.Due <= DateTime.UtcNow).Select(kv => kv.Key);
        var dueCards = cardRepository.GetCardsForDeck(deckId).Where(c => dueCardIds.Contains(c.Id));

        var studiedCardIds = _scheduler.GetCardInfos(userId).Keys;
        var newCards = cardRepository.GetCardsForDeck(deckId).Where(c => !studiedCardIds.Contains(c.Id));

        return dueCards.Concat(newCards).OrderBy(c => c.CreatedOn);
    }

    public Task Handle(IMessageContext context, CardStatusChanged @event)
    {
        _scheduler.ReviewCard(@event.Creator, @event.CardId, @event.Status switch
        {
            CardStatus.Again => FreeSpacedRepetitionScheduler.Rating.Again,
            // CardStatus.Hard => FreeSpacedRepetitionScheduler.Rating.Hard,
            CardStatus.Good => FreeSpacedRepetitionScheduler.Rating.Good,
            CardStatus.Easy => FreeSpacedRepetitionScheduler.Rating.Easy,
            _ => throw new ArgumentOutOfRangeException(nameof(@event.Status), "Invalid card rating")
        }, context.ConsumerContext.MessageTimestamp);
        return Task.CompletedTask;
    }
}