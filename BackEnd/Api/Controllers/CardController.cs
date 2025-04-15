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
    public IActionResult CreateCard([FromBody] CardAddModel model)
    {
        commandBus.Send(new CreateCardCommand(model.DeckId, model.Front, model.Back));
        return Ok();
    }

    [HttpPut("{cardId:guid}")]
    public IActionResult UpdateCard([FromRoute] Guid cardId, [FromBody] CardUpdateModel model, [FromServices] CardProjection cardProjection)
    {
        commandBus.Send(new UpdateCardCommand(cardId, model.Front, model.Back));
        return Ok();
    }

    [HttpPatch("{cardId:guid}")]
    public IActionResult ChangeCardStatus([FromRoute] Guid cardId, [FromBody] CardStatusModel model)
    {
        var status = model.Choice switch
        {
            "again" => CardStatus.Again,
            "good" => CardStatus.Good,
            "easy" => CardStatus.Easy,
            _ => CardStatus.Again
        };

        commandBus.Send(new ChangeCardStatus(cardId, status));
        return Ok();
    }

    [HttpDelete("{cardId:guid}")]
    public IActionResult DeleteCard([FromRoute] Guid cardId)
    {
        commandBus.Send(new DeleteCardCommand(cardId));
        return Ok();
    }
}