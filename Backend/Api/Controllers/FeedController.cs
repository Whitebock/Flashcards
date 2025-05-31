using Flashcards.Api.Models;
using Flashcards.Api.Projections;
using Flashcards.Common.Projections;
using Flashcards.Common.UserManagement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flashcards.Api.Controllers;

[ApiController]
[Route("feed")]
public class FeedController(IUserStore userStore, IProjection<FeedProjection> feedProjection, IProjection<DecksProjection> decksProjection) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [EndpointSummary("Get Feed")]
    [EndpointDescription("Returns a list of entries for the global feed.")]
    [ProducesResponseType<IEnumerable<Activity>>(StatusCodes.Status200OK, "application/json")]
    public async Task<IActionResult> GetGlobalFeed()
    {
        var feedList = await feedProjection.GetAsync();
        var decks = await decksProjection.GetAsync();
        Stack<Activity> feed = [];
        foreach (var activity in feedList.GetGlobalFeed())
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
                DeckName = decks.GetDeck(activity.DeckId)?.Name ?? string.Empty,
                Count = activity.Count
            });
        }
        return Ok(feed);
    }
}