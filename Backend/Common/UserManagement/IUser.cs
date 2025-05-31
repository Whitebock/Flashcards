namespace Flashcards.Common.UserManagement;

public interface IUser
{
    Guid Id { get; }
    string Username { get; }
    string Name { get; }
    string? Picture { get; }
}