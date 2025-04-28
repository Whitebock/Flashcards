
namespace Flashcards.Common.UserManagement;

public class DeletedUserStore(IUserStore decorated) : IUserStore
{
    public async Task<IUser> GetById(Guid userId)
    {
        try
        {
            return await decorated.GetById(userId);
        }
        catch (Exception)
        {
            return SystemUser;
        }
    }

    public async Task<IUser> GetByUsername(string username)
    {
        try
        {
            return await decorated.GetByUsername(username);
        }
        catch (Exception)
        {
            return SystemUser;
        }
    }

    private IUser SystemUser => new User(){
        Id = Guid.Parse("dc66143b-fb07-421e-9f9f-1ff8e8c87f2c"),
        Username = "System"
    };
}