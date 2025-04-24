using Microsoft.AspNetCore.Mvc;
using Flashcards.Api.Projections;
using Microsoft.AspNetCore.Authorization;
using Flashcards.Common.UserManagement;
using Flashcards.Common.ServiceBus;
using Flashcards.Common.Messages;
using Flashcards.Common.Messages.Commands;

namespace Flashcards.Api.Controllers;

[ApiController]
[Authorize("HasUser")]
[Route("decks")]
public class DeckController(
    DeckListProjection deckListProjection, 
    CardProjection cardProjection, 
    ICommandBus commandBus
    ) : ControllerBase
{

    [HttpGet]
    [AllowAnonymous]
    [Route("reccomended")]
    public IActionResult GetReccomendedDecks()
    {
        var decks = deckListProjection.GetAllDecks();
        
        return Ok(new {
            popular = decks,
            recent = decks
        });
    }

    [HttpGet]
    public IActionResult GetAllDecks()
    {
        var decks = deckListProjection.GetAllDecks();
        return Ok(decks);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchDecksAsync([FromQuery] string username, [FromQuery] string deckname, [FromServices] IUserStore userStore)
    {
        var user = await userStore.GetByUsername(username);
        var deck = deckListProjection.GetAllDecks()
            //.Where(d => d.Creator == user.Id)
            .Where(d => d.EncodedName.Equals(deckname))
            .FirstOrDefault();
        return Ok(deck);
    }

    [HttpGet("{deckId:guid}/study")]
    public IActionResult GetStudyCard([FromRoute] Guid deckId)
    {
        var cards = cardProjection
            .GetCardsForDeck(deckId)
            .Where(c => c.Status == CardStatus.NotSeen || c.Status == CardStatus.Again)
            .OrderBy(c => c.Status);

        var card = cards.FirstOrDefault();
        var left = cards.Count();

        return Ok(new { card, left });
    }

    [HttpGet("{deckId:guid}")]
    public IActionResult GetDeck([FromRoute] Guid deckId)
    {
        var deck = deckListProjection.GetDeck(deckId);
        return Ok(deck);
    }

    [HttpGet("{deckId:guid}/cards")]
    public IActionResult GetCardsForDeck([FromRoute] Guid deckId)
    {
        var cards = cardProjection.GetCardsForDeck(deckId);
        return Ok(cards);
    }

    public record DeckUpdateModel(string Name, string Description);
    
    [HttpPost]
    public async Task<IActionResult> CreateDeckAsync([FromBody] DeckUpdateModel model)
    {
        await commandBus.SendAsync(new CreateDeckCommand(model.Name, model.Description) 
        {
             Creator = User.GetAppId()
        });
        return Ok();
    }

    [HttpPut("{deckId:guid}")]
    public async Task<IActionResult> UpdateDeckAsync([FromRoute] Guid deckId, [FromBody] DeckUpdateModel model)
    {
        await commandBus.SendAsync(new UpdateDeckCommand(deckId, model.Name, model.Description) 
        {
             Creator = User.GetAppId()
        });
        return Ok();
    }

    [HttpDelete("{deckId:guid}")]
    public async Task<IActionResult> DeleteDeckAsync([FromRoute] Guid deckId)
    {
        await commandBus.SendAsync(new DeleteDeckCommand(deckId) 
        {
             Creator = User.GetAppId()
        });
        return Ok();
    }
}
