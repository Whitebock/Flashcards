
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

    public async Task<IUser?> GetById(Guid userId)
    {
        var users = await _apiClient.Users.GetAllAsync(new GetUsersRequest() {
            Query = $"app_metadata.id: {userId}"
        });
        return users.Count != 1 ? null : ConvertToUser(users.First());
    }

    public async Task<IUser?> GetByUsername(string username)
    {
        var users = await _apiClient.Users.GetAllAsync(new GetUsersRequest() {
            Query = $"username: {username}"
        });
        return users.Count != 1 ? null : ConvertToUser(users.First());
    }

    private static IUser ConvertToUser(Auth0.ManagementApi.Models.User auth0User)
    {
        return new User()
        {
            Id = auth0User.AppMetadata.id,
            Username = auth0User.UserName,
            Picture = auth0User.Picture
        };
    }
}