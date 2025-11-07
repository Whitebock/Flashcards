using System.Collections.Concurrent;
using Flashcards.Common.Interfaces;
using Flashcards.Common.Messages.Events;
using KafkaFlow;

namespace Flashcards.Common.Infrastructure.Consumers;

public class DecksProjection(ICardRepository cardRepository) :
    IMessageHandler<DeckCreated>,
    IMessageHandler<DeckUpdated>,
    IMessageHandler<DeckDeleted>,
    IMessageHandler<TagAdded>,
    IMessageHandler<TagRemoved>,
    IMessageHandler<CardCreated>,
    IMessageHandler<CardDeleted>, 
    IDeckRepository
{
    public class DeckDto
    {
        public Guid DeckId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CardCount { get; set; } = 0;
        public List<string> Tags { get; set; } = [];
        public Guid CreatorId { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }

    private readonly ConcurrentDictionary<Guid, DeckDto> _decks = [];

    public DeckDto? GetDeck(Guid deckId)
    {
        return _decks.TryGetValue(deckId, out var deck) ? deck : null;
    }

    public IEnumerable<DeckDto> GetAllDecks()
    {
        return _decks.Values;
    }

    public Task Handle(IMessageContext context, DeckCreated @event)
    {
        _decks.TryAdd(@event.DeckId, new DeckDto()
        {
            DeckId = @event.DeckId,
            Name = @event.Name,
            Description = @event.Description,
            CreatorId = @event.Creator,
        });
        return Task.CompletedTask;
    }

    public Task Handle(IMessageContext context, DeckUpdated @event)
    {
        _decks[@event.DeckId].Name = @event.Name;
        _decks[@event.DeckId].Description = @event.Description;
        _decks[@event.DeckId].LastUpdated = DateTime.Now;
        return Task.CompletedTask;
    }

    public Task Handle(IMessageContext context, DeckDeleted @event)
    {
        _decks.Remove(@event.DeckId, out _);
        return Task.CompletedTask;
    }

    public Task Handle(IMessageContext context, TagAdded @event)
    {
        _decks[@event.DeckId].Tags.Add(@event.Tag);
        _decks[@event.DeckId].LastUpdated = DateTime.Now;
        return Task.CompletedTask;
    }

    public Task Handle(IMessageContext context, TagRemoved @event)
    {
        _decks[@event.DeckId].Tags.Remove(@event.Tag);
        _decks[@event.DeckId].LastUpdated = DateTime.Now;
        return Task.CompletedTask;
    }

    public Task Handle(IMessageContext context, CardCreated @event)
    {
        _decks[@event.DeckId].CardCount++;
        _decks[@event.DeckId].LastUpdated = DateTime.Now;
        return Task.CompletedTask;
    }

    public Task Handle(IMessageContext context, CardDeleted @event)
    {
        var deckId = cardRepository.GetDeckForCard(@event.CardId);
        _decks[deckId].CardCount--;
        _decks[deckId].LastUpdated = DateTime.Now;
        return Task.CompletedTask;
    }
}