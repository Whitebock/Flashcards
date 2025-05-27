using Flashcards.Common.Messages;
using Flashcards.Common.Messages.Events;

namespace Flashcards.Api.Projections;

public class CardProjection : 
    IEventHandler<CardCreated>,
    IEventHandler<CardUpdated>,
    IEventHandler<CardDeleted>,
    IEventHandler<CardStatusChanged>
{
    public class CardDto
    {
        public required Guid Id { get; set; }
        public required Guid DeckId { get; set; }
        public required string Front { get; set; }
        public required string Back { get; set; }
        public CardStatus Status { get; set; } = CardStatus.NotSeen;
    };
    private readonly Dictionary<Guid, CardDto> _cards = [];

    public IEnumerable<CardDto> GetCardsForDeck(Guid deckId) => _cards.Values.Where(c => c.DeckId == deckId);
    public Guid GetDeckForCard(Guid cardId) =>_cards[cardId].DeckId;

    public void Handle(CardCreated e)
    {
        _cards.Add(e.CardId, new CardDto()
        {
            Id = e.CardId,
            DeckId = e.DeckId,
            Front = e.Front,
            Back = e.Back
        });
    }

    public void Handle(CardUpdated @event)
    {
        _cards[@event.CardId].Front = @event.Front;
        _cards[@event.CardId].Front = @event.Back;
    }

    public void Handle(CardDeleted @event)
    {
        _cards.Remove(@event.CardId);
    }

    public void Handle(CardStatusChanged @event)
    {
        _cards[@event.CardId].Status = @event.Status;
    }
}