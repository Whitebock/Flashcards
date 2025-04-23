
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.Extensions.Options;

namespace Flashcards.Common.UserManagement;

public class Auth0UserStore : IUserStore
{
    private ManagementApiClient _apiClient;
    public Auth0UserStore(IOptions<Auth0UserStoreOptions> options)
    {
        _apiClient = new ManagementApiClient(options.Value.Token, options.Value.Endpoint);
    }

    public Task<IUser> GetById(Guid userId)
    {
        throw new NotImplementedException();
    }

    public async Task<IUser> GetByUsername(string username)
    {
        var users = await _apiClient.Users.GetAllAsync(new GetUsersRequest() {
            Query = $"username: {username}"
        });
        if (users.Count != 1)
        {
            throw new InvalidOperationException($"Expected exactly one user with username '{username}', but found {users.Count}.");
        }
        var auth0User = users.First();
        return new User() {
            Id = auth0User.AppMetadata.id,
            Username = auth0User.UserName
        };
    }
}