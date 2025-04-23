namespace Flashcards.Common.UserManagement;

public interface IUserStore
{
    Task<IUser> GetById(Guid userId);
    Task<IUser> GetByUsername(string username);
}