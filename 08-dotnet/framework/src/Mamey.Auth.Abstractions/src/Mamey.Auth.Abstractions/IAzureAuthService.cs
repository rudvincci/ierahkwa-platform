using Microsoft.Graph.Models;

namespace Mamey.Auth.Abstractions;

/// <summary>
/// Interface for Azure AD (B2C/B2B) authentication and user management services.
/// </summary>
public interface IAzureAuthService
{
    Task<string> AcquireTokenAsync(string[] scopes);

    Task<bool> VerifyUserActionAsync(string userId, string token);

    Task<bool> ValidateIdTokenAsync(string idToken);

    Task<string> RefreshAccessTokenAsync(string refreshToken);

    Task<bool> DeleteUserAsync(string userId);

    Task<User?> UpdateUserAsync(string userId, User updatedUser);

    Task<bool> DisableUserAsync(string userId);

    Task<bool> EnableUserAsync(string userId);

    Task<Dictionary<string, string>> GetUserClaimsAsync(string userId);

    Task<bool> SetUserClaimsAsync(string userId, Dictionary<string, string> claims);

    Task<bool> AddUserToGroupAsync(string userId, string groupId);

    Task<bool> RemoveUserFromGroupAsync(string userId, string groupId);
    
    Task<bool> LogoutUserAsync(string userId);
    
    Task<List<string>> ListGroupsForUserAsync(string userId);

    Task<UserCollectionResponse> SearchUsersAsync(string query);

    Task<bool> ForcePasswordChangeAsync(string userId);
    
    Task<bool> LockUserAccountAsync(string userId);

    Task<bool> InviteUserAsync(string email, string displayName);

    Task<string> GenerateSignInUrlAsync(string redirectUrl, string state, string nonce);

    Task<bool> ValidateConfigurationAsync();
}