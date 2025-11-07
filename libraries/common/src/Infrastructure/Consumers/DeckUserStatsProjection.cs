using Flashcards.Common.Interfaces;
using Flashcards.Common.Messages;
using Flashcards.Common.Messages.Events;
using KafkaFlow;

namespace Flashcards.Common.Infrastructure.Consumers;

public class DeckUserStatsProjection(ICardRepository cardRepository) :
    IMessageHandler<CardStatusChanged>, IDeckUserStatsRepository
{
    public class DeckStatDto
    {
        public int NotSeen { get; set; } = 0;
        public int Correct { get; set; } = 0;
        public int Incorrect { get; set; } = 0;
    }

    private readonly Dictionary<Guid, Dictionary<Guid, DeckStatDto>> _userDeckStats = [];

    public DeckStatDto? GetDeckStats(Guid userId, Guid deckId)
    {
        return _userDeckStats.GetValueOrDefault(userId)?.GetValueOrDefault(deckId);
    }

    public Task Handle(IMessageContext context, CardStatusChanged @event)
    {
        var deckId = cardRepository.GetDeckForCard(@event.CardId);
        var userId = @event.Creator;

        if (!_userDeckStats.ContainsKey(userId))
            _userDeckStats[userId] = [];

        if (!_userDeckStats[userId].ContainsKey(deckId))
            _userDeckStats[userId][deckId] = new DeckStatDto() { NotSeen = cardRepository.GetCardsForDeck(deckId).Count() };

        var stats = _userDeckStats[userId][deckId];
        stats.NotSeen--;
        switch (@event.Status)
        {
            case CardStatus.Again:
                stats.Incorrect++;
                break;
            case CardStatus.Good:
            case CardStatus.Easy:
                stats.Correct++;
                break;
            case CardStatus.NotSeen:
                stats.NotSeen++;
                break;
        }
        return Task.CompletedTask;
    }
}