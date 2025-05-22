using Flashcards.Api.Models;
using Flashcards.Common.Messages;
using Flashcards.Common.Messages.Events;
using Flashcards.Common.Projections;

namespace Flashcards.Api.Projections;

public class DeckStatsProjection(IProjection<CardProjection> _cardProjection) : 
    IEventHandler<DeckCreated>,
    IEventHandler<DeckUpdated>,
    IEventHandler<DeckDeleted>,
    IEventHandler<CardCreated>,
    IEventHandler<CardStatusChanged>,
    IEventHandler<TagAdded>,
    IEventHandler<TagRemoved>
{
    public List<Deck> _decks = [];

    public Task HandleAsync(DeckCreated @event)
    {
        _decks.Add(new Deck()
        {
            Id = @event.DeckId,
            Name = @event.Name,
            Description = @event.Description,
            CreatorId = @event.Creator
        });
        return Task.CompletedTask;
    }

    public Task HandleAsync(DeckUpdated @event)
    {
        var deck = _decks.FirstOrDefault(d => d.Id.Equals(@event.DeckId));
        if (deck != null) {
            deck.Name = @event.Name;
            deck.Description = @event.Description;
        }
        return Task.CompletedTask;
    }

    public Task HandleAsync(DeckDeleted @event)
    {
        var deck = _decks.FirstOrDefault(d => d.Id.Equals(@event.DeckId));
        if(deck != null) _decks.Remove(deck);
        return Task.CompletedTask;
    }

    public IEnumerable<Deck> GetAllDecks() => _decks;
    public Deck GetDeck(Guid deckId) => _decks.First(d => d.Id.Equals(deckId));

    public async Task HandleAsync(CardCreated @event)
    {
        var cards = await _cardProjection.GetAsync();
        var deckId = cards.GetDeckForCard(@event.CardId);
        await UpdateDeckStats(deckId);
    }

    public async Task HandleAsync(CardStatusChanged @event)
    {
        var cards = await _cardProjection.GetAsync();
        var deckId = cards.GetDeckForCard(@event.CardId);
        await UpdateDeckStats(deckId);
    }

    private async Task UpdateDeckStats(Guid deckId) {
        var deck = _decks.First(deck => deck.Id.Equals(deckId));
        var cards = (await _cardProjection.GetAsync()).GetCardsForDeck(deckId);
        deck.Statistics = new DeckStatistics() {
            NotSeen = cards.Count(c => c.Status == CardStatus.NotSeen),
            Correct = cards.Count(c => c.Status == CardStatus.Good || c.Status == CardStatus.Easy),
            Incorrect = cards.Count(c => c.Status == CardStatus.Again)
        };
    }

    public Task HandleAsync(TagAdded @event)
    {
        var deck = _decks.First(deck => deck.Id.Equals(@event.DeckId));
        deck.Tags.Add(@event.Tag);
        return Task.CompletedTask;
    }

    public Task HandleAsync(TagRemoved @event)
    {
        var deck = _decks.First(deck => deck.Id.Equals(@event.DeckId));
        deck.Tags.Remove(@event.Tag);
        return Task.CompletedTask;
    }
}