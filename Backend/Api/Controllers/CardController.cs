using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Flashcards.Common.ServiceBus;
using Flashcards.Common.Messages.Commands;
using Flashcards.Common.Messages;
using Flashcards.Api.Models;

namespace Flashcards.Api.Controllers;

[ApiController]
[Authorize("HasUser")]
[Route("cards")]
public class CardController(ICommandBus commandBus) : ControllerBase
{    
    [HttpPost]
    [EndpointSummary("Create Card")]
    public async Task<IActionResult> CreateCardAsync([FromBody] Card card)
    {
        await commandBus.SendAsync(new CreateCardCommand(card.DeckId.Value, card.Front, card.Back) 
        {
             Creator = User.GetAppId()
        });
        return Ok();
    }

    [HttpPut("{cardId:guid}")]
    [EndpointSummary("Update Card")]
    public async Task<IActionResult> UpdateCardAsync([FromRoute] Guid cardId, [FromBody] Card card)
    {
        await commandBus.SendAsync(new UpdateCardCommand(cardId, card.Front, card.Back) 
        {
             Creator = User.GetAppId()
        });
        return Ok();
    }

    [HttpPatch("{cardId:guid}")]
    [EndpointSummary("Change Card Status")]
    public async Task<IActionResult> ChangeCardStatusAsync([FromRoute] Guid cardId, [FromBody] Card card)
    {
        await commandBus.SendAsync(new ChangeCardStatus(cardId, card.Status.Value) 
        {
             Creator = User.GetAppId()
        });
        return Ok();
    }

    [HttpDelete("{cardId:guid}")]
    [EndpointSummary("Delete Card")]
    public async Task<IActionResult> DeleteCardAsync([FromRoute] Guid cardId)
    {
        await commandBus.SendAsync(new DeleteCardCommand(cardId) 
        {
             Creator = User.GetAppId()
        });
        return Ok();
    }
}