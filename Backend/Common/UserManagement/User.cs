
namespace Flashcards.Common.UserManagement;

public class User : IUser
{
    public required Guid Id { get; set; }

    public required string Username { get; set; }

    public string? Picture { get; set; }
}