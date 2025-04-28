using Flashcards.Common.UserManagement;
using Microsoft.AspNetCore.Mvc;

namespace Flashcards.Api.Controllers;

[ApiController]
[Route("users")]
public class UserController(IUserStore userStore) : ControllerBase
{
    [HttpGet]
    [Route("{username}")]
    [EndpointSummary("Get by Username")]
    [ProducesResponseType<User>(StatusCodes.Status200OK, "application/json")]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound, "application/problem+json")]
    public async Task<IActionResult> GetByUsername([FromRoute] string username)
    {
        var user = await userStore.GetByUsername(username);
        if(user == null) {
            return NotFound();
        }

        return Ok(user);
    }
}