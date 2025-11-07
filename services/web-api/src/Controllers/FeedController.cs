using Flashcards.Api.Models;
using Flashcards.Common.Infrastructure.Consumers;
using Flashcards.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flashcards.Api.Controllers;

[ApiController]
[Route("feed")]
public class FeedController(IUserStore userStore, IFeedRepository feedRepository, IDeckRepository deckRepository) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [EndpointSummary("Get Feed")]
    [EndpointDescription("Returns a list of entries for the global feed.")]
    [ProducesResponseType<IEnumerable<Activity>>(StatusCodes.Status200OK, "application/json")]
    public async Task<IActionResult> GetGlobalFeed()
    {
        Stack<Activity> feed = [];
        foreach (var activity in feedRepository.GetGlobalFeed())
        {
            var user = await userStore.GetById(activity.UserId);
            if(user == null) continue;
            feed.Push(new Activity()
            {
                Type = activity.Type switch
                {
                    FeedProjection.ActivityType.DeckCreated => "deck_added",
                    FeedProjection.ActivityType.DeckEdited => "deck_edited",
                    FeedProjection.ActivityType.CardAdded => "cards_added",
                    _ => throw new ArgumentOutOfRangeException(nameof(activity.Type), activity.Type, null)
                },
                OccurredAt = activity.Timestamp,
                User = user,
                DeckName = deckRepository.GetDeck(activity.DeckId)?.Name ?? string.Empty,
                Count = activity.Count
            });
        }
        return Ok(feed);
    }
}