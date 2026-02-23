using AppBuilder.Core.Models;

namespace AppBuilder.Core.Interfaces;

/// <summary>Auth - JWT, email/password, social login (Google, Facebook, GitHub).</summary>
public interface IAuthService
{
    (User User, string Token) Register(string email, string password, string? name = null);
    (User? User, string? Token) Login(string email, string password);
    User? GetUserById(string id);
    User? GetUserByEmail(string email);
    void UpdateLastLogin(string userId);
    (User? User, string? Token) SocialLogin(string provider, string providerId, string email, string? name, string? avatarUrl);
    bool TryConsumeBuildCredit(string userId);
    void GrantBuildCredits(string userId, int count);
    bool DeleteUser(string userId);
    int GetUserCount();
}
