using Flashcards.Common.Messages;
using Flashcards.Common.Messages.Events;

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

    public Task HandleAsync(CardCreated e)
    {
        _cards.Add(new CardDto(e.CardId, e.Front, e.Back), e.DeckId);
        return Task.CompletedTask;
    }

    public Task HandleAsync(CardUpdated @event)
    {
        var cardToUpdate = _cards.FirstOrDefault(c => c.Key.Id == @event.CardId);
        if (cardToUpdate.Key != null)
        {
            _cards.Remove(cardToUpdate.Key);
            _cards.Add(new CardDto(@event.CardId, @event.Front, @event.Back), cardToUpdate.Value);
        }
        return Task.CompletedTask;
    }

    public Task HandleAsync(CardDeleted @event)
    {
        var cardToRemove = _cards.FirstOrDefault(c => c.Key.Id == @event.CardId);
        _cards.Remove(cardToRemove.Key);
        return Task.CompletedTask;
    }

    public Task HandleAsync(CardStatusChanged @event)
    {
        var cardToUpdate = _cards.FirstOrDefault(c => c.Key.Id == @event.CardId);
        if (cardToUpdate.Key != null)
        {
            _cards.Remove(cardToUpdate.Key);
            _cards.Add(cardToUpdate.Key with {Status = @event.Status}, cardToUpdate.Value);
        }
        return Task.CompletedTask;
    }
}