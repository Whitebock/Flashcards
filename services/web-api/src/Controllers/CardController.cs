using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Flashcards.Api.Models;
using Flashcards.Common.Interfaces;

namespace Flashcards.Api.Controllers;

[ApiController]
[Authorize("HasUser")]
[Route("cards")]
public class CardController(ICardManager cardManager) : ControllerBase
{    
    [HttpPost]
    [EndpointSummary("Create Card")]
    public async Task<IActionResult> CreateCardAsync([FromBody] Card card)
    {
        if (card.DeckId == null || card.Front == null || card.Back == null) return BadRequest();
        await cardManager.CreateAsync(User.GetAppId(), card.DeckId.Value, card.Front, card.Back);
        return Ok();
    }

    [HttpPut("{cardId:guid}")]
    [EndpointSummary("Update Card")]
    public async Task<IActionResult> UpdateCardAsync([FromRoute] Guid cardId, [FromBody] Card card)
    {
        if (card.Front == null || card.Back == null) return BadRequest();
        await cardManager.UpdateAsync(User.GetAppId(), cardId, card.Front, card.Back);
        return Ok();
    }

    [HttpPatch("{cardId:guid}")]
    [EndpointSummary("Change Card Status")]
    public async Task<IActionResult> ChangeCardStatusAsync([FromRoute] Guid cardId, [FromBody] Card card)
    {
        if (card.Status == null) return BadRequest("Status is required.");
        await cardManager.ChangeStatusAsync(User.GetAppId(), cardId, card.Status.Value);
        return Ok();
    }

    [HttpDelete("{cardId:guid}")]
    [EndpointSummary("Delete Card")]
    public async Task<IActionResult> DeleteCardAsync([FromRoute] Guid cardId)
    {
        await cardManager.DeleteAsync(User.GetAppId(), cardId);
        return Ok();
    }
}