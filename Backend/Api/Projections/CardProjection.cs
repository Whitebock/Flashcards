using Flashcards.CQRS;
using Flashcards.Events;

namespace Flashcards.Api.Projections;

public class CardProjection : 
    IEventHandler<CardCreated>,
    IEventHandler<CardUpdated>,
    IEventHandler<CardDeleted>,
    IEventHandler<CardStatusChanged>
{
    public record CardDto(Guid Id, string Front, string Back, CardStatus Status = CardStatus.NotSeen);
    private Dictionary<CardDto, Guid> _cards = [];

    public List<CardDto> GetCardsForDeck(Guid id) => 
        _cards.Where(c => c.Value == id).Select(c => c.Key).ToList();
    
    public Guid GetDeckForCard(Guid id) =>
        _cards.FirstOrDefault(c => c.Key.Id == id).Value;

    public void Handle(CardCreated e)
    {
        _cards.Add(new CardDto(e.CardId, e.Front, e.Back), e.DeckId);
    }

    public void Handle(CardUpdated @event)
    {
        var cardToUpdate = _cards.FirstOrDefault(c => c.Key.Id == @event.CardId);
        if (cardToUpdate.Key != null)
        {
            _cards.Remove(cardToUpdate.Key);
            _cards.Add(new CardDto(@event.CardId, @event.Front, @event.Back), cardToUpdate.Value);
        }
    }

    public void Handle(CardDeleted @event)
    {
        var cardToRemove = _cards.FirstOrDefault(c => c.Key.Id == @event.CardId);
        _cards.Remove(cardToRemove.Key);
    }

    public void Handle(CardStatusChanged @event)
    {
        var cardToUpdate = _cards.FirstOrDefault(c => c.Key.Id == @event.CardId);
        if (cardToUpdate.Key != null)
        {
            _cards.Remove(cardToUpdate.Key);
            _cards.Add(cardToUpdate.Key with {Status = @event.Status}, cardToUpdate.Value);
        }
    }
}