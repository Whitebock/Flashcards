

namespace Flashcards.Common.UserManagement;

public class Auth0User(Auth0.ManagementApi.Models.User auth0User) : IUser
{
    public Guid Id => auth0User.AppMetadata.id;

    public string Username => auth0User.UserName;

    public string Name => auth0User.FullName;

    public string? Picture => auth0User.Picture;
}