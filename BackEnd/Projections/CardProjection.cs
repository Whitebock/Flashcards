using Flashcards.CQRS;
using Flashcards.Events;

namespace Flashcards.Projections;

public class CardProjection : IEventHandler<DeckCreated>, IEventHandler<CardCreated>
{
    public record CardDto(string Front, string Back);
    private Dictionary<Guid, List<CardDto>> _decks = [];

    public void Handle(DeckCreated e)
    {
        _decks.Add(e.Id, []);
    }

    public void Handle(CardCreated e)
    {
        _decks[e.DeckId].Add(new CardDto(e.Front, e.Back));
    }

    public List<CardDto> GetCards(Guid id) => _decks[id];
}