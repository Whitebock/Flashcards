using System.Threading.Tasks;
using Flashcards.Common.UserManagement;
using Microsoft.AspNetCore.Mvc;

namespace Flashcards.Api.Controllers;

[ApiController]
[Route("users")]
public class UserController(IUserStore userStore) : ControllerBase
{
    [HttpGet]
    [Route("{username}")]
    public async Task<IActionResult> GetByUsername([FromRoute] string username)
    {
        var user = await userStore.GetByUsername(username);
        if(user == null) {
            return NotFound(new { Message = "User not found" });
        }

        return Ok(user);
    }
}