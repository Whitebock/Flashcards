using Flashcards.Common.Messages;
using Flashcards.Common.Messages.Events;

namespace Flashcards.ContentUtil;

public class DeckListProjection() :
    IAsyncEventHandler<DeckCreated>,
    IAsyncEventHandler<DeckUpdated>,
    IAsyncEventHandler<DeckDeleted>
{
    public Dictionary<Guid, string> Decks { get; set; } = [];
    public Task HandleAsync(DeckCreated @event)
    {
        Decks.Add(@event.DeckId, @event.Name);
        return Task.CompletedTask;
    }

    public Task HandleAsync(DeckUpdated @event)
    {
        Decks[@event.DeckId] = @event.Name;
        return Task.CompletedTask;
    }

    public Task HandleAsync(DeckDeleted @event)
    {
        Decks.Remove(@event.DeckId);
        return Task.CompletedTask;
    }
}
