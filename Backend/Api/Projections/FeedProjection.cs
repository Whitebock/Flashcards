using Flashcards.Common.Messages;
using Flashcards.Common.Messages.Events;

namespace Flashcards.Api.Projections;

public class FeedProjection() :
    IEventHandler<DeckCreated>,
    IEventHandler<DeckUpdated>,
    IEventHandler<CardCreated>
{
    public class FeedActivityDto
    {
        public required DateTime Timestamp { get; set; }
        public required ActivityType Type { get; set; }
        public required Guid UserId { get; set; }

        public required Guid DeckId { get; set; }
        public int Count { get; set; } = 1;
    }

    public enum ActivityType
    {
        DeckCreated,
        DeckEdited,
        CardAdded
    }

    private readonly List<FeedActivityDto> _globalFeed = [];

    public IEnumerable<FeedActivityDto> GetGlobalFeed() => _globalFeed.OrderBy(dto => dto.Timestamp);

    public void Handle(DeckCreated @event)
    {
        _globalFeed.Add(new FeedActivityDto()
        {
            Type = ActivityType.DeckCreated,
            Timestamp = @event.Timestamp,
            UserId = @event.Creator,
            DeckId = @event.DeckId
        });
    }

    public void Handle(DeckUpdated @event)
    {
        _globalFeed.Add(new FeedActivityDto()
        {
            Type = ActivityType.DeckEdited,
            Timestamp = @event.Timestamp,
            UserId = @event.Creator,
            DeckId = @event.DeckId
        });
    }

    public void Handle(CardCreated @event)
    {
        var activity = _globalFeed
            .Where(dto => dto.UserId == @event.Creator && dto.DeckId == @event.DeckId)
            .Where(dto => dto.Type == ActivityType.CardAdded)
            .FirstOrDefault();

        if (activity != null)
        {
            activity.Count++;
            activity.Timestamp = @event.Timestamp;
        }
        else
        {
            _globalFeed.Add(new FeedActivityDto()
            {
                Timestamp = @event.Timestamp,
                Type = ActivityType.CardAdded,
                UserId = @event.Creator,
                DeckId = @event.DeckId
            });
        }
    }
}