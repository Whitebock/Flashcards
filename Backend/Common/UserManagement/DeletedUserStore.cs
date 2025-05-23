
using Flashcards.Common.Projections;

namespace Flashcards.Common.UserManagement;

public class DeletedUserStore(IUserStore decorated, IProjection<UserIdProjection> projection) : IUserStore
{
    public async Task<IUser?> GetById(Guid userId)
    {
        var user = await decorated.GetById(userId);
        var userIds = await projection.GetAsync();
        if (user == null && userIds.HasUserId(userId))
        {
            // This user has been deleted.
            return DeletedUser;
        }
        return user;
    }

    public async Task<IUser?> GetByUsername(string username)
    {
        var user = await decorated.GetByUsername(username);
        if(user == null && username == DeletedUser.Username) {
            // This user has been deleted.
            return DeletedUser;
        }
        return user;
    }

    private IUser DeletedUser => new User(){
        Id = Guid.Empty,
        Username = "Deleted"
    };
}