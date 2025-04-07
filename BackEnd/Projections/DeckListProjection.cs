using Flashcards.CQRS;
using Flashcards.Events;

namespace Flashcards.Projections;

public class DeckListProjection : IEventHandler<DeckCreated>, IEventHandler<CardCreated>
{
    public record DeckStatDto(int NotSeen = 0, int Correct = 0, int Incorrect = 0) {
        public int Total  => NotSeen + Correct + Incorrect;
    }
    public record DeckDto(Guid Id, string Name) {
        public DeckStatDto Stats { get; set; } = new();
    }
    public List<DeckDto> Decks { get; } = [];

    public void Handle(DeckCreated e)
    {
        Decks.Add(new DeckDto(e.Id, e.Name));
    }

    public void Handle(CardCreated e)
    {
        var deck = Decks.First(deck => deck.Id == e.DeckId);
        deck.Stats = deck.Stats with { NotSeen = deck.Stats.NotSeen + 1 };
    }
}