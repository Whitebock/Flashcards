using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Flashcards.Api.Models;
using Flashcards.Common.Infrastructure.Consumers;
using Flashcards.Common.Interfaces;

namespace Flashcards.Api.Controllers;

[ApiController]
[Authorize("HasUser")]
[Route("decks")]
public class DeckController(
    IDeckRepository deckRepository,
    IDeckActivityRepository deckActivityRepository,
    IDeckUserStatsRepository deckUserStatsRepository,
    ICardRepository cardRepository,
    IRepetitionAlgorithm repetitionAlgorithm,
    IDeckManager  deckManager,
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
        var decks = deckRepository.GetAllDecks();
        if(query.User != null) {
            var user = await userStore.GetByUsername(query.User);
            if(user == null) 
                return NotFound("User was not found");
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
                decks = decks.OrderByDescending(deck => deckActivityRepository.GetDeckActivity(deck.DeckId));
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
            Creator = creator,
            Tags = [.. dto.Tags]
        };
        if (User.Identity?.IsAuthenticated == true)
        {
            var deckStats = deckUserStatsRepository.GetDeckStats(User.GetAppId(), dto.DeckId);
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
        var deck = deckRepository.GetDeck(deckId);
        if (deck == null) return NotFound();
        return Ok(await DeckDtoToModel(deck));
    }

    [HttpGet("{deckId:guid}/cards")]
    [AllowAnonymous]
    [EndpointSummary("Cards of a Deck")]
    public async Task<ActionResult<IEnumerable<Card>>> GetCardsForDeckAsync([FromRoute] Guid deckId)
    {
        var cards = cardRepository.GetCardsForDeck(deckId)
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
        await deckManager.CreateAsync(User.GetAppId(), deck.Name, deck.Description);
        return Ok();
    }

    [HttpPut("{deckId:guid}")]
    [EndpointSummary("Update Deck")]
    public async Task<IActionResult> UpdateDeckAsync([FromRoute] Guid deckId, [FromBody] Deck deck)
    {
        if(deck.Name == null || deck.Description == null) return BadRequest();
        await deckManager.UpdateAsync(User.GetAppId(), deckId, deck.Name, deck.Description);
        return Ok();
    }

    [HttpDelete("{deckId:guid}")]
    [EndpointSummary("Delete Deck")]
    public async Task<IActionResult> DeleteDeckAsync([FromRoute] Guid deckId)
    {
        await deckManager.DeleteAsync(User.GetAppId(), deckId);
        return Ok();
    }

    public record StudyDeckResponse(Card card, int left);

    [HttpGet("{deckId:guid}/study")]
    [EndpointSummary("Study a Deck")]
    public async Task<ActionResult<StudyDeckResponse>> GetStudyCardAsync([FromRoute] Guid deckId)
    {
        var userId = User.GetAppId();
        var toStudy = await repetitionAlgorithm.GetStudyBatchAsync(userId, deckId);

        var card = toStudy.FirstOrDefault();
        var left = toStudy.Count();

        if(card == null) return NotFound();

        return Ok(new StudyDeckResponse(new Card() {
            Id = card.Id,
            Front = card.Front,
            Back = card.Back,
            Status = card.Status
        }, left));
    }
}
