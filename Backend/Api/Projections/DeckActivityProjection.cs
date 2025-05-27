using Flashcards.Common.Messages;
using Flashcards.Common.Messages.Events;
using Flashcards.Common.Projections;

namespace Flashcards.Api.Projections;

public class DeckActivityProjection(IProjection<CardProjection> _cardProjection) :
    IAsyncEventHandler<CardStatusChanged>
{
    private readonly Dictionary<Guid, int> _deckActivity = [];

    public async Task HandleAsync(CardStatusChanged @event)
    {
        var cards = await _cardProjection.GetAsync();
        var deckId = cards.GetDeckForCard(@event.CardId);

        if (!_deckActivity.ContainsKey(deckId))
            _deckActivity[deckId] = 0;

        _deckActivity[deckId]++;
    }

    public int GetDeckActivity(Guid deckId)
    {
        return _deckActivity.GetValueOrDefault(deckId);
    }
}