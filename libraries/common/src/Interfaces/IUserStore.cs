namespace Flashcards.Common.Interfaces;

public interface IUserStore
{
    Task<IUser?> GetById(Guid userId);
    Task<IUser?> GetByUsername(string username);
}