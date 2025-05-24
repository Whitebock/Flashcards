
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.Extensions.Options;

namespace Flashcards.Common.UserManagement;

public class Auth0UserStore(IOptions<Auth0UserStoreOptions> options, IHttpClientFactory clientFactory) : IUserStore
{
    private DateTime _tokenTime = DateTime.MinValue;
    private ManagementApiClient? _apiClient;

    private async Task<ManagementApiClient> GetApiClient()
    {
        // Refresh access token every hour.
        if (_tokenTime < DateTime.Now.AddHours(-1))
        {
            var opts = options.Value;
            var content = new StringContent(JsonSerializer.Serialize(new
            {
                client_id = opts.ClientId,
                client_secret = opts.ClientSecret,
                audience = $"https://{opts.Domain}/api/v2/",
                grant_type = "client_credentials"
            }), Encoding.UTF8, "application/json");

            var httpClient = clientFactory.CreateClient();
            var response = await httpClient.PostAsync($"https://{opts.Domain}/oauth/token", content);
            response.EnsureSuccessStatusCode();

            var tokenResponse = await response.Content.ReadFromJsonAsync<AccessTokenResponse>() ??
                throw new InvalidOperationException("Failed to retrieve access token.");

            if (_apiClient == null)
            {
                _apiClient = new ManagementApiClient(tokenResponse.Token, opts.Domain);
            }
            else
            {
                _apiClient.UpdateAccessToken(tokenResponse.Token);
            }
            _tokenTime = DateTime.Now;
        }
        return _apiClient!;
    }

    public async Task<IUser?> GetById(Guid userId)
    {
        var api = await GetApiClient();
        var users = await api.Users.GetAllAsync(new GetUsersRequest()
        {
            Query = $"app_metadata.id: {userId}"
        });
        return users.Count != 1 ? null : ConvertToUser(users.First());
    }

    public async Task<IUser?> GetByUsername(string username)
    {
        var api = await GetApiClient();
        var users = await api.Users.GetAllAsync(new GetUsersRequest() {
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

    private class AccessTokenResponse
    {
        [JsonPropertyName("access_token")]
        public required string Token { get; set; }

        [JsonPropertyName("scope")]
        public required string Scope { get; set; }

        [JsonPropertyName("expires_in")]
        public required int ExpiresIn { get; set; }
    }
}