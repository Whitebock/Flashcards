using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Flashcards.Common.ServiceBus;
using Flashcards.Common.Messages.Commands;
using Flashcards.Api.Models;

namespace Flashcards.Api.Controllers;

[ApiController]
[Authorize("HasUser")]
[Route("cards")]
public class CardController(ICommandSender commandSender) : ControllerBase
{    
    [HttpPost]
    [EndpointSummary("Create Card")]
    public async Task<IActionResult> CreateCardAsync([FromBody] Card card)
    {
        if (card.DeckId == null || card.Front == null || card.Back == null) return BadRequest();
        await commandSender.SendAsync(new CreateCardCommand(card.DeckId.Value, card.Front, card.Back)
        {
            Creator = User.GetAppId()
        });
        return Ok();
    }

    [HttpPut("{cardId:guid}")]
    [EndpointSummary("Update Card")]
    public async Task<IActionResult> UpdateCardAsync([FromRoute] Guid cardId, [FromBody] Card card)
    {
        if (card.Front == null || card.Back == null) return BadRequest();
        await commandSender.SendAsync(new UpdateCardCommand(cardId, card.Front, card.Back)
        {
            Creator = User.GetAppId()
        });
        return Ok();
    }

    [HttpPatch("{cardId:guid}")]
    [EndpointSummary("Change Card Status")]
    public async Task<IActionResult> ChangeCardStatusAsync([FromRoute] Guid cardId, [FromBody] Card card)
    {
        if (card.Status == null) return BadRequest("Status is required.");
        await commandSender.SendAsync(new ChangeCardStatusCommand(cardId, card.Status.Value)
        {
            Creator = User.GetAppId()
        });
        return Ok();
    }

    [HttpDelete("{cardId:guid}")]
    [EndpointSummary("Delete Card")]
    public async Task<IActionResult> DeleteCardAsync([FromRoute] Guid cardId)
    {
        await commandSender.SendAsync(new DeleteCardCommand(cardId) 
        {
             Creator = User.GetAppId()
        });
        return Ok();
    }
}