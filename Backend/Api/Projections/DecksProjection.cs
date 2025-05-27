using Flashcards.Common.Messages;
using Flashcards.Common.Messages.Events;
using Flashcards.Common.Projections;

namespace Flashcards.Api.Projections;

public class DecksProjection(IProjection<CardProjection> cardProjection) :
    IEventHandler<DeckCreated>,
    IEventHandler<DeckUpdated>,
    IEventHandler<DeckDeleted>,
    IEventHandler<TagAdded>,
    IEventHandler<TagRemoved>,
    IEventHandler<CardCreated>,
    IAsyncEventHandler<CardDeleted>
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

    private readonly Dictionary<Guid, DeckDto> _decks = [];

    public DeckDto? GetDeck(Guid deckId)
    {
        return _decks.TryGetValue(deckId, out var deck) ? deck : null;
    }

    public IEnumerable<DeckDto> GetAllDecks()
    {
        return _decks.Values;
    }

    public void Handle(DeckCreated @event)
    {
        _decks.Add(@event.DeckId, new DeckDto()
        {
            DeckId = @event.DeckId,
            Name = @event.Name,
            Description = @event.Description,
            CreatorId = @event.Creator,
        });
    }

    public void Handle(DeckUpdated @event)
    {
        _decks[@event.DeckId].Name = @event.Name;
        _decks[@event.DeckId].Description = @event.Description;
        _decks[@event.DeckId].LastUpdated = DateTime.Now;
    }

    public void Handle(DeckDeleted @event)
    {
        _decks.Remove(@event.DeckId);
    }

    public void Handle(TagAdded @event)
    {
        _decks[@event.DeckId].Tags.Add(@event.Tag);
        _decks[@event.DeckId].LastUpdated = DateTime.Now;
    }

    public void Handle(TagRemoved @event)
    {
        _decks[@event.DeckId].Tags.Remove(@event.Tag);
        _decks[@event.DeckId].LastUpdated = DateTime.Now;
    }

    public void Handle(CardCreated @event)
    {
        _decks[@event.DeckId].CardCount++;
        _decks[@event.DeckId].LastUpdated = DateTime.Now;
    }

    public async Task HandleAsync(CardDeleted @event)
    {
        var cards = await cardProjection.GetAsync();
        var deckId = cards.GetDeckForCard(@event.CardId);
        _decks[deckId].CardCount--;
        _decks[deckId].LastUpdated = DateTime.Now;
    }
}