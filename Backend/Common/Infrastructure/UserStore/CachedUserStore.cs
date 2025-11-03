
using Flashcards.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Flashcards.Common.Infrastructure.UserStore;

public class CachedUserStore(IUserStore decorated, IMemoryCache cache) : IUserStore
{
    public async Task<IUser?> GetById(Guid userId)
    {
        return await cache.GetOrCreateAsync("user/" + userId, async (entry) => {
            return await decorated.GetById(userId);
        });
    }

    public async Task<IUser?> GetByUsername(string username)
    {
        return await cache.GetOrCreateAsync("user/" + username, async (entry) => {
            return await decorated.GetByUsername(username);
        });
    }
}