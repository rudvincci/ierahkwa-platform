using System.Security.Claims;
using Mamey.Identity.Core;

namespace Mamey.Identity.Azure.Abstractions;

/// <summary>
/// Azure authentication service interface.
/// </summary>
public interface IAzureAuthService
{
    /// <summary>
    /// Gets the current authenticated user from Azure AD.
    /// </summary>
    /// <param name="claimsPrincipal">The claims principal.</param>
    /// <returns>The authenticated user.</returns>
    Task<AuthenticatedUser?> GetAuthenticatedUserAsync(ClaimsPrincipal claimsPrincipal);

    /// <summary>
    /// Gets user information from Microsoft Graph.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>User information from Graph.</returns>
    Task<AzureUserInfo?> GetUserInfoAsync(string userId);

    /// <summary>
    /// Gets user groups from Microsoft Graph.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>User groups.</returns>
    Task<IEnumerable<AzureGroupInfo>> GetUserGroupsAsync(string userId);

    /// <summary>
    /// Validates an Azure AD token.
    /// </summary>
    /// <param name="token">The token to validate.</param>
    /// <returns>True if valid, false otherwise.</returns>
    Task<bool> ValidateTokenAsync(string token);

    /// <summary>
    /// Signs out the user from Azure AD.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <returns>Sign out URL.</returns>
    Task<string> GetSignOutUrlAsync(string userId);
}

/// <summary>
/// Azure user information from Microsoft Graph.
/// </summary>
public class AzureUserInfo
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string GivenName { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Mail { get; set; } = string.Empty;
    public string UserPrincipalName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string OfficeLocation { get; set; } = string.Empty;
    public string MobilePhone { get; set; } = string.Empty;
    public string BusinessPhones { get; set; } = string.Empty;
    public DateTime? CreatedDateTime { get; set; }
    public DateTime? LastPasswordChangeDateTime { get; set; }
    public bool AccountEnabled { get; set; }
    public string PreferredLanguage { get; set; } = string.Empty;
}

/// <summary>
/// Azure group information from Microsoft Graph.
/// </summary>
public class AzureGroupInfo
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Mail { get; set; } = string.Empty;
    public string MailNickname { get; set; } = string.Empty;
    public bool SecurityEnabled { get; set; }
    public bool MailEnabled { get; set; }
    public DateTime? CreatedDateTime { get; set; }
}
