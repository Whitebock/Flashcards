using Flashcards.Common.Messages;
using Flashcards.Common.Messages.Events;

namespace Flashcards.Common.Projections;

/// <summary>
/// Used for determining if a user ever interacted with the application.
/// </summary>
public class UserIdProjection() :
    IEventHandler<DeckCreated>,
    IEventHandler<CardStatusChanged>
{
    private readonly HashSet<Guid> _userIds = [];

    public Task HandleAsync(DeckCreated @event)
    {
        _userIds.Add(@event.Creator);
        return Task.CompletedTask;
    }

    public Task HandleAsync(CardStatusChanged @event)
    {
        _userIds.Add(@event.Creator);
        return Task.CompletedTask;
    }

    public bool HasUserId(Guid userId)
    {
        return _userIds.Contains(userId);
    }
}