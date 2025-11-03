using Flashcards.Common.Interfaces;
using Flashcards.Common.Messages.Events;
using KafkaFlow;

namespace Flashcards.Common.Infrastructure.Consumers;

public class FeedProjection() :
    IMessageHandler<DeckCreated>,
    IMessageHandler<DeckUpdated>,
    IMessageHandler<CardCreated>, IFeedRepository
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

    public Task Handle(IMessageContext context, DeckCreated @event)
    {
        _globalFeed.Add(new FeedActivityDto()
        {
            Type = ActivityType.DeckCreated,
            Timestamp = @event.Timestamp,
            UserId = @event.Creator,
            DeckId = @event.DeckId
        });
        return Task.CompletedTask;
    }

    public Task Handle(IMessageContext context, DeckUpdated @event)
    {
        _globalFeed.Add(new FeedActivityDto()
        {
            Type = ActivityType.DeckEdited,
            Timestamp = @event.Timestamp,
            UserId = @event.Creator,
            DeckId = @event.DeckId
        });
        return Task.CompletedTask;
    }

    public Task Handle(IMessageContext context, CardCreated @event)
    {
        var activity = _globalFeed
            .Where(dto => dto.UserId == @event.Creator && dto.DeckId == @event.DeckId)
            .FirstOrDefault(dto => dto.Type == ActivityType.CardAdded);

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
        return Task.CompletedTask;
    }
}