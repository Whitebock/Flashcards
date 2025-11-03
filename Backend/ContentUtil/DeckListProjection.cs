using Flashcards.Common.Messages.Events;
using KafkaFlow;

namespace Flashcards.ContentUtil;

public class DeckListProjection() :
    IMessageHandler<DeckCreated>,
    IMessageHandler<DeckUpdated>,
    IMessageHandler<DeckDeleted>
{
    public Dictionary<Guid, string> Decks { get; set; } = [];

    public Task Handle(IMessageContext context, DeckCreated @event)
    {
        Decks.Add(@event.DeckId, @event.Name);
        return Task.CompletedTask;    }

    public Task Handle(IMessageContext context, DeckUpdated @event)
    {
        Decks[@event.DeckId] = @event.Name;
        return Task.CompletedTask;    }

    public Task Handle(IMessageContext context, DeckDeleted @event)
    {
        Decks.Remove(@event.DeckId);
        return Task.CompletedTask;    }
}
