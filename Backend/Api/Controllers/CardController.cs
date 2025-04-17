using Microsoft.AspNetCore.Mvc;
using System;
using Flashcards.Commands;
using Flashcards.CQRS;
using Flashcards.Api.Projections;

namespace Flashcards.Api.Controllers;

[ApiController]
[Route("cards")]
public class CardController(ICommandBus commandBus) : ControllerBase
{
    public record CardAddModel(Guid DeckId, string Front, string Back);
    public record CardUpdateModel(string Front, string Back);
    public record CardStatusModel(string Choice);
    
    [HttpPost]
    public async Task<IActionResult> CreateCardAsync([FromBody] CardAddModel model)
    {
        await commandBus.SendAsync(new CreateCardCommand(model.DeckId, model.Front, model.Back));
        return Ok();
    }

    [HttpPut("{cardId:guid}")]
    public async Task<IActionResult> UpdateCardAsync([FromRoute] Guid cardId, [FromBody] CardUpdateModel model, [FromServices] CardProjection cardProjection)
    {
        await commandBus.SendAsync(new UpdateCardCommand(cardId, model.Front, model.Back));
        return Ok();
    }

    [HttpPatch("{cardId:guid}")]
    public async Task<IActionResult> ChangeCardStatusAsync([FromRoute] Guid cardId, [FromBody] CardStatusModel model)
    {
        var status = model.Choice switch
        {
            "again" => CardStatus.Again,
            "good" => CardStatus.Good,
            "easy" => CardStatus.Easy,
            _ => CardStatus.Again
        };

        await commandBus.SendAsync(new ChangeCardStatus(cardId, status));
        return Ok();
    }

    [HttpDelete("{cardId:guid}")]
    public async Task<IActionResult> DeleteCardAsync([FromRoute] Guid cardId)
    {
        await commandBus.SendAsync(new DeleteCardCommand(cardId));
        return Ok();
    }
}