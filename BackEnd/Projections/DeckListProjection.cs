using Flashcards.CQRS;
using Flashcards.Events;

namespace Flashcards.Projections;

public class DeckListProjection(CardProjection _cardProjection) : 
    IEventHandler<DeckCreated>,
    IEventHandler<DeckUpdated>,
    IEventHandler<DeckDeleted>,
    IEventHandler<CardStatusChanged>
{
    public record DeckStatDto(int NotSeen = 0, int Correct = 0, int Incorrect = 0) {
        public int Total  => NotSeen + Correct + Incorrect;
    }
    public record DeckDto(Guid Id) {
        public required string Name { get; set; }
        public DeckStatDto Stats { get; set; } = new();
    }
    public List<DeckDto> _decks = [];

    public void Handle(DeckCreated e)
    {
        _decks.Add(new DeckDto(e.DeckId) { Name = e.Name });
    }

    public void Handle(DeckUpdated @event)
    {
        var deck = _decks.FirstOrDefault(d => d.Id.Equals(@event.DeckId));
        if (deck != null) deck.Name = @event.Name;
    }

    public void Handle(DeckDeleted @event)
    {
        var deck = _decks.FirstOrDefault(d => d.Id.Equals(@event.DeckId));
        if(deck != null) _decks.Remove(deck);
    }

    public List<DeckDto> GetAllDecks() => _decks;

    public void Handle(CardStatusChanged @event)
    {
        var deckId = _cardProjection.GetDeckForCard(@event.CardId);
        var deck = _decks.First(deck => deck.Id.Equals(deckId));
        var cards = _cardProjection.GetCardsForDeck(deckId);
        deck.Stats = new DeckStatDto() {
            NotSeen = cards.Count(c => c.Status == CardStatus.NotSeen),
            Correct = cards.Count(c => c.Status == CardStatus.Good || c.Status == CardStatus.Easy),
            Incorrect = cards.Count(c => c.Status == CardStatus.Again)
        };
    }
}