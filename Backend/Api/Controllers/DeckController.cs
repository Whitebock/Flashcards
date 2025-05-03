using Microsoft.AspNetCore.Mvc;
using Flashcards.Api.Projections;
using Microsoft.AspNetCore.Authorization;
using Flashcards.Common.UserManagement;
using Flashcards.Common.ServiceBus;
using Flashcards.Common.Messages;
using Flashcards.Common.Messages.Commands;
using Flashcards.Api.Models;

namespace Flashcards.Api.Controllers;

[ApiController]
[Authorize("HasUser")]
[Route("decks")]
public class DeckController(
    DeckListProjection deckListProjection, 
    CardProjection cardProjection, 
    ICommandBus commandBus,
    IUserStore userStore
    ) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [EndpointSummary("All Decks")]
    public ActionResult<IEnumerable<Deck>> GetAllDecks()
    {
        var decks = deckListProjection.GetAllDecks();
        return Ok(decks);
    }

    public class ReccomendedDecksResponse()
    {
        public required IEnumerable<Deck> Popular { get; init; }
        public required IEnumerable<Deck> Recent { get; init; }
    };
    [HttpGet]
    [AllowAnonymous]
    [Route("recommended")]
    [EndpointSummary("Recommended Decks")]
    [EndpointDescription("Returns the most popular and recently created decks.")]
    public async Task<ActionResult<ReccomendedDecksResponse>> GetRecommendedDecksAsync()
    {
        var decks = deckListProjection.GetAllDecks();
        await AddCreatorName([..decks]);
        
        return Ok(new ReccomendedDecksResponse() {
            Popular = decks,
            Recent = decks,
        });
    }

    [HttpGet("search")]
    [AllowAnonymous]
    [EndpointSummary("Search Decks")]
    public async Task<ActionResult<IEnumerable<Deck>>> SearchDecksAsync([FromQuery] string? username, [FromQuery] string? deckname)
    {
        IEnumerable<Deck> decks = deckListProjection.GetAllDecks();
        if(username != null) {
            var user = await userStore.GetByUsername(username);
            if(user == null) return NotFound("User was not found");
            decks = decks.Where(d => d.CreatorId.Equals(user.Id));
        }
        if(deckname != null) {
            decks = decks.Where(d => d.EncodedName.Equals(deckname));
        }
        await AddCreatorName([..decks]);
        return Ok(decks);
    }

    [HttpGet("{deckId:guid}")]
    [AllowAnonymous]
    [EndpointSummary("Deck Info")]
    public async Task<ActionResult<Deck>> GetDeckAsync([FromRoute] Guid deckId)
    {
        var deck = deckListProjection.GetDeck(deckId);
        await AddCreatorName(deck);
        return Ok(deck);
    }

    private async Task AddCreatorName(params Deck[] decks) {
        foreach (var deck in decks)
        {
            var user = await userStore.GetById(deck.CreatorId!.Value);
            if(user != null) deck.CreatorName = user.Username;
        }
    }

    [HttpGet("{deckId:guid}/cards")]
    [EndpointSummary("Cards of a Deck")]
    public ActionResult<IEnumerable<Card>> GetCardsForDeck([FromRoute] Guid deckId)
    {
        var cards = cardProjection.GetCardsForDeck(deckId)
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
        await commandBus.SendAsync(new CreateDeckCommand(deck.Name, deck.Description) 
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
        await commandBus.SendAsync(new UpdateDeckCommand(deckId, deck.Name, deck.Description) 
        {
             Creator = User.GetAppId()
        });
        return Ok();
    }

    [HttpDelete("{deckId:guid}")]
    [EndpointSummary("Delete Deck")]
    public async Task<IActionResult> DeleteDeckAsync([FromRoute] Guid deckId)
    {
        await commandBus.SendAsync(new DeleteDeckCommand(deckId) 
        {
             Creator = User.GetAppId()
        });
        return Ok();
    }

    public record StudyDeckResponse(Card card, int left);

    [HttpGet("{deckId:guid}/study")]
    [EndpointSummary("Study a Deck")]
    public ActionResult<StudyDeckResponse> GetStudyCard([FromRoute] Guid deckId)
    {
        var cards = cardProjection
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
