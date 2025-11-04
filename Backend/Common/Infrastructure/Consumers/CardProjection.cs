using Flashcards.Common.Interfaces;
using Flashcards.Common.Messages;
using Flashcards.Common.Messages.Events;
using KafkaFlow;

namespace Flashcards.Common.Infrastructure.Consumers;

public class CardProjection : 
    IMessageHandler<CardCreated>,
    IMessageHandler<CardUpdated>,
    IMessageHandler<CardDeleted>,
    IMessageHandler<CardStatusChanged>, 
    ICardRepository
{
    public class CardDto
    {
        public required Guid Id { get; set; }
        public required Guid DeckId { get; set; }
        public required string Front { get; set; }
        public required string Back { get; set; }
        public required DateTime CreatedOn { get; set; }
        public CardStatus Status { get; set; } = CardStatus.NotSeen;
    };
    private readonly Dictionary<Guid, CardDto> _cards = [];

    public IEnumerable<CardDto> GetCardsForDeck(Guid deckId) => _cards.Values.Where(c => c.DeckId == deckId);
    public Guid GetDeckForCard(Guid cardId) => _cards[cardId].DeckId;

    public Task Handle(IMessageContext context, CardCreated e)
    {
        _cards.Add(e.CardId, new CardDto()
        {
            Id = e.CardId,
            DeckId = e.DeckId,
            Front = e.Front,
            Back = e.Back,
            CreatedOn = context.ConsumerContext.MessageTimestamp
        });
        return Task.CompletedTask;
    }

    public Task Handle(IMessageContext context, CardUpdated @event)
    {
        _cards[@event.CardId].Front = @event.Front;
        _cards[@event.CardId].Back = @event.Back;
        return Task.CompletedTask;
    }

    public Task Handle(IMessageContext context, CardDeleted @event)
    {
        _cards.Remove(@event.CardId);
        return Task.CompletedTask;
    }

    public Task Handle(IMessageContext context, CardStatusChanged @event)
    {
        _cards[@event.CardId].Status = @event.Status;
        return Task.CompletedTask;
    }
}