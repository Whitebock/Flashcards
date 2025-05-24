namespace Flashcards.Common.UserManagement;

public class Auth0UserStoreOptions
{
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? Domain { get; set; }
}