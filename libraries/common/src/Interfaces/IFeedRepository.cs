using Flashcards.Common.Infrastructure.Consumers;

namespace Flashcards.Common.Interfaces;

public interface IFeedRepository
{
    IEnumerable<FeedProjection.FeedActivityDto> GetGlobalFeed();
}