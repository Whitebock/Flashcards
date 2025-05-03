using Flashcards.Api.Models;
using Flashcards.Common.Messages;
using Flashcards.Common.Messages.Events;

namespace Flashcards.Api.Projections;

public class DeckListProjection(CardProjection _cardProjection) : 
    IEventHandler<DeckCreated>,
    IEventHandler<DeckUpdated>,
    IEventHandler<DeckDeleted>,
    IEventHandler<CardCreated>,
    IEventHandler<CardStatusChanged>
{
    public List<Deck> _decks = [];

    public void Handle(DeckCreated @event)
    {
        _decks.Add(new Deck() {
            Id = @event.DeckId,
            Name = @event.Name,
            Description = @event.Description,
            CreatorId = @event.Creator
        });
    }

    public void Handle(DeckUpdated @event)
    {
        var deck = _decks.FirstOrDefault(d => d.Id.Equals(@event.DeckId));
        if (deck != null) {
            deck.Name = @event.Name;
            deck.Description = @event.Description;
        }
    }

    public void Handle(DeckDeleted @event)
    {
        var deck = _decks.FirstOrDefault(d => d.Id.Equals(@event.DeckId));
        if(deck != null) _decks.Remove(deck);
    }

    public List<Deck> GetAllDecks() => _decks;
    public Deck GetDeck(Guid deckId) => _decks.First(d => d.Id.Equals(deckId));

    public void Handle(CardCreated @event)
    {
        var deckId = _cardProjection.GetDeckForCard(@event.CardId);
        UpdateDeckStats(deckId);
    }

    public void Handle(CardStatusChanged @event)
    {
        var deckId = _cardProjection.GetDeckForCard(@event.CardId);
        UpdateDeckStats(deckId);
    }

    private void UpdateDeckStats(Guid deckId) {
        var deck = _decks.First(deck => deck.Id.Equals(deckId));
        var cards = _cardProjection.GetCardsForDeck(deckId);
        deck.Statistics = new DeckStatistics() {
            NotSeen = cards.Count(c => c.Status == CardStatus.NotSeen),
            Correct = cards.Count(c => c.Status == CardStatus.Good || c.Status == CardStatus.Easy),
            Incorrect = cards.Count(c => c.Status == CardStatus.Again)
        };
    }
}