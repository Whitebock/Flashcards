using System.Security.Claims;

namespace Flashcards.Api.Controllers;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetAppId(this ClaimsPrincipal user)
    {
        var id = user?.FindFirst("app_id")?.Value;
        if (Guid.TryParse(id, out Guid result))
        {
            return result;
        }
        throw new InvalidOperationException("The app_id claim is missing or invalid.");
    }
}