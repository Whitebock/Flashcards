using System.Collections.Concurrent;
using Flashcards.Common.Interfaces;
using Flashcards.Common.Messages.Events;
using KafkaFlow;

namespace Flashcards.Common.Infrastructure.Consumers;

public class DeckActivityProjection(ICardRepository cardRepository) :
    IMessageHandler<CardStatusChanged>, IDeckActivityRepository
{
    private readonly ConcurrentDictionary<Guid, int> _deckActivity = [];

    public int GetDeckActivity(Guid deckId)
    {
        return _deckActivity.GetValueOrDefault(deckId);
    }

    public Task Handle(IMessageContext context, CardStatusChanged @event)
    {
        var deckId = cardRepository.GetDeckForCard(@event.CardId);

        _deckActivity.TryAdd(deckId, 0);

        _deckActivity[deckId]++;
        return Task.CompletedTask;
    }
}