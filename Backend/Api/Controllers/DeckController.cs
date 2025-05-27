using Microsoft.AspNetCore.Mvc;
using Flashcards.Api.Projections;
using Microsoft.AspNetCore.Authorization;
using Flashcards.Common.UserManagement;
using Flashcards.Common.ServiceBus;
using Flashcards.Common.Messages;
using Flashcards.Common.Messages.Commands;
using Flashcards.Api.Models;
using Flashcards.Common.Projections;

namespace Flashcards.Api.Controllers;

[ApiController]
[Authorize("HasUser")]
[Route("decks")]
public class DeckController(
    IProjection<DecksProjection> decksProjection,
    IProjection<DeckActivityProjection> deckACtivityProjection,
    IProjection<DeckUserStatsProjection> deckUserStatsProjection,
    IProjection<CardProjection> cardProjection, 
    ICommandSender commandSender,
    IUserStore userStore
    ) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [EndpointSummary("Get Decks")]
    [EndpointDescription("Returns a list of decks. You can filter by user and name.")]
    [ProducesResponseType<IEnumerable<Deck>>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    public async Task<ActionResult<IEnumerable<Deck>>> SearchDecksAsync([FromQuery] DeckQuery query)
    {
        var deckList = await decksProjection.GetAsync();
        var decks = deckList.GetAllDecks();
        if(query.User != null) {
            var user = await userStore.GetByUsername(query.User);
            if(user == null) return NotFound("User was not found");
            decks = decks.Where(d => d.CreatorId.Equals(user.Id));
        }
        if(query.Name != null) {
            decks = decks.Where(d => d.Name.ToLower().Replace(' ', '_').Equals(query.Name));
        }
        switch (query.Sort)
        {
            case "last-updated":
                decks = decks.OrderByDescending(deck => deck.LastUpdated);
                break;
            case "new":
                decks = decks.OrderByDescending(deck => deck.Created);
                break;
            case "popular":
                var activity = await deckACtivityProjection.GetAsync();
                decks = decks.OrderByDescending(deck => activity.GetDeckActivity(deck.DeckId));
                break;
            default:
                return BadRequest("Unknown sorting option: " + query.Sort);
        }
        decks = decks.Take(query.Amount);
        var result = await Task.WhenAll(decks.Select(DeckDtoToModel));
        return Ok(result);
    }

    private async Task<Deck> DeckDtoToModel(DecksProjection.DeckDto dto)
    {
        var creator = await userStore.GetById(dto.CreatorId);
        var deck = new Deck()
        {
            Id = dto.DeckId,
            Name = dto.Name,
            Description = dto.Description,
            CardCount = dto.CardCount,
            CreatorId = dto.CreatorId,
            CreatorName = creator?.Username,
            Tags = [.. dto.Tags]
        };
        if (User.Identity?.IsAuthenticated == true)
        {
            var stats = await deckUserStatsProjection.GetAsync();
            var deckStats = stats.GetDeckStats(User.GetAppId(), dto.DeckId);
            deck.Statistics = deckStats == null ? null : new DeckStatistics()
            {
                NotSeen = deckStats.NotSeen,
                Correct = deckStats.Correct,
                Incorrect = deckStats.Incorrect
            };
        }
        return deck;
    }

    [HttpGet("{deckId:guid}")]
    [AllowAnonymous]
    [EndpointSummary("Deck Info")]
    public async Task<ActionResult<Deck>> GetDeckAsync([FromRoute] Guid deckId)
    {
        var projection = await decksProjection.GetAsync();
        var deck = projection.GetDeck(deckId);
        if (deck == null) return NotFound();
        return Ok(await DeckDtoToModel(deck));
    }

    [HttpGet("{deckId:guid}/cards")]
    [AllowAnonymous]
    [EndpointSummary("Cards of a Deck")]
    public async Task<ActionResult<IEnumerable<Card>>> GetCardsForDeckAsync([FromRoute] Guid deckId)
    {
        var projection = await cardProjection.GetAsync();
        var cards = projection.GetCardsForDeck(deckId)
            .Select(card => new Card() {
                Id = card.Id,
                Front = card.Front,
                Back = card.Back,
                Status = card.Status,
                DeckId = deckId
            });
        return Ok(cards);
    }
    
    [HttpPost]
    [EndpointSummary("Create Deck")]
    public async Task<IActionResult> CreateDeckAsync([FromBody] Deck deck)
    {
        if(deck.Name == null || deck.Description == null) return BadRequest();
        await commandSender.SendAsync(new CreateDeckCommand(deck.Name, deck.Description) 
        {
             Creator = User.GetAppId()
        });
        return Ok();
    }

    [HttpPut("{deckId:guid}")]
    [EndpointSummary("Update Deck")]
    public async Task<IActionResult> UpdateDeckAsync([FromRoute] Guid deckId, [FromBody] Deck deck)
    {
        if(deck.Name == null || deck.Description == null) return BadRequest();
        await commandSender.SendAsync(new UpdateDeckCommand(deckId, deck.Name, deck.Description) 
        {
             Creator = User.GetAppId()
        });
        return Ok();
    }

    [HttpDelete("{deckId:guid}")]
    [EndpointSummary("Delete Deck")]
    public async Task<IActionResult> DeleteDeckAsync([FromRoute] Guid deckId)
    {
        await commandSender.SendAsync(new DeleteDeckCommand(deckId) 
        {
             Creator = User.GetAppId()
        });
        return Ok();
    }

    public record StudyDeckResponse(Card card, int left);

    [HttpGet("{deckId:guid}/study")]
    [EndpointSummary("Study a Deck")]
    public async Task<ActionResult<StudyDeckResponse>> GetStudyCardAsync([FromRoute] Guid deckId)
    {
        var projection = await cardProjection.GetAsync();
        var cards = projection
            .GetCardsForDeck(deckId)
            .Where(c => c.Status == CardStatus.NotSeen || c.Status == CardStatus.Again)
            .OrderBy(c => c.Status);

        var card = cards.FirstOrDefault();
        var left = cards.Count();

        if(card == null) return NotFound();

        return Ok(new StudyDeckResponse(new Card() {
            Id = card.Id,
            Front = card.Front,
            Back = card.Back,
            Status = card.Status
        }, left));
    }
}
