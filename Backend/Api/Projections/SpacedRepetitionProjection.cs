using Flashcards.Common.Messages;
using Flashcards.Common.Messages.Events;
using Flashcards.Common.Projections;

namespace Flashcards.Api.Projections;
public class SpacedRepetitionProjection(IProjection<CardProjection> cardProjection) :
    IEventHandler<CardStatusChanged>
{
    private readonly FreeSpacedRepetitionScheduler scheduler = new();

    public void Handle(CardStatusChanged @event)
    {
        scheduler.ReviewCard(@event.Creator, @event.CardId, @event.Status switch
        {
            CardStatus.Again => FreeSpacedRepetitionScheduler.Rating.Again,
            // CardStatus.Hard => FreeSpacedRepetitionScheduler.Rating.Hard,
            CardStatus.Good => FreeSpacedRepetitionScheduler.Rating.Good,
            CardStatus.Easy => FreeSpacedRepetitionScheduler.Rating.Easy,
            _ => throw new ArgumentOutOfRangeException(nameof(@event.Status), "Invalid card rating")
        }, @event.Timestamp);
    }

    public async Task<IEnumerable<CardProjection.CardDto>> GetStudyBatchAsync(Guid userId, Guid deckId)
    {
        var cards = await cardProjection.GetAsync();

        var dueCardIds = scheduler.GetCardInfos(userId).Where(kv => kv.Value.Due <= DateTime.UtcNow).Select(kv => kv.Key);
        var dueCards = cards.GetCardsForDeck(deckId).Where(c => dueCardIds.Contains(c.Id));

        var studiedCardIds = scheduler.GetCardInfos(userId).Keys;
        var newCards = cards.GetCardsForDeck(deckId).Where(c => !studiedCardIds.Contains(c.Id));

        return dueCards.Concat(newCards).OrderBy(c => c.CreatedOn);
    }

}