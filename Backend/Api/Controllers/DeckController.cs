using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using Flashcards.Commands;
using Flashcards.CQRS;
using Flashcards.Api.Projections;

namespace Flashcards.Api.Controllers;

[ApiController]
[Route("decks")]
public class DeckController(
    DeckListProjection deckListProjection, 
    CardProjection cardProjection, 
    ICommandBus commandBus
    ) : ControllerBase
{
    public record DeckUpdateModel(string Name, string Description);

    [HttpGet]
    public IActionResult GetAllDecks()
    {
        var decks = deckListProjection.GetAllDecks();
        return Ok(decks);
    }

    [HttpGet("search")]
    public IActionResult SearchDecks([FromQuery] string user, [FromQuery] string name)
    {
        var deck = deckListProjection.GetAllDecks()
            .Where(d => d.FriendlyId.Equals(name))
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

    [HttpPost]
    public async Task<IActionResult> CreateDeckAsync([FromBody] DeckUpdateModel model)
    {
        await commandBus.SendAsync(new CreateDeckCommand(model.Name, model.Description));
        return Ok();
    }

    [HttpPut("{deckId:guid}")]
    public async Task<IActionResult> UpdateDeckAsync([FromRoute] Guid deckId, [FromBody] DeckUpdateModel model)
    {
        await commandBus.SendAsync(new UpdateDeckCommand(deckId, model.Name, model.Description));
        return Ok();
    }

    [HttpDelete("{deckId:guid}")]
    public async Task<IActionResult> DeleteDeckAsync([FromRoute] Guid deckId)
    {
        await commandBus.SendAsync(new DeleteDeckCommand(deckId));
        return Ok();
    }
}
