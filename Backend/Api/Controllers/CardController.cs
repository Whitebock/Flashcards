using Microsoft.AspNetCore.Mvc;
using Flashcards.Commands;
using Flashcards.CQRS;
using Flashcards.Api.Projections;
using Microsoft.AspNetCore.Authorization;
using Api.Controllers;

namespace Flashcards.Api.Controllers;

[ApiController]
[Authorize("HasUser")]
[Route("cards")]
public class CardController(ICommandBus commandBus) : ControllerBase
{
    public record CardAddModel(Guid DeckId, string Front, string Back);
    public record CardUpdateModel(string Front, string Back);
    public record CardStatusModel(string Choice);
    
    [HttpPost]
    public async Task<IActionResult> CreateCardAsync([FromBody] CardAddModel model)
    {
        await commandBus.SendAsync(new CreateCardCommand(model.DeckId, model.Front, model.Back) 
        {
             Creator = User.GetAppId()
        });
        return Ok();
    }

    [HttpPut("{cardId:guid}")]
    public async Task<IActionResult> UpdateCardAsync([FromRoute] Guid cardId, [FromBody] CardUpdateModel model)
    {
        await commandBus.SendAsync(new UpdateCardCommand(cardId, model.Front, model.Back) 
        {
             Creator = User.GetAppId()
        });
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

        await commandBus.SendAsync(new ChangeCardStatus(cardId, status) 
        {
             Creator = User.GetAppId()
        });
        return Ok();
    }

    [HttpDelete("{cardId:guid}")]
    public async Task<IActionResult> DeleteCardAsync([FromRoute] Guid cardId)
    {
        await commandBus.SendAsync(new DeleteCardCommand(cardId) 
        {
             Creator = User.GetAppId()
        });
        return Ok();
    }
}