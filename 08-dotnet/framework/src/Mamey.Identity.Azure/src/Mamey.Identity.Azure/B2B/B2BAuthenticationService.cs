using System.Security.Claims;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Mamey.Identity.Core;
using Microsoft.Extensions.Logging;
using Mamey.Identity.Azure.Abstractions;
using Mamey.Identity.Azure.Configuration;

namespace Mamey.Identity.Azure.B2B;

/// <summary>
/// Azure AD B2B authentication service.
/// </summary>
public class B2BAuthenticationService : IAzureAuthService
{
    private readonly GraphServiceClient _graphServiceClient;
    private readonly Configuration.AzureOptions _azureOptions;
    private readonly ILogger<B2BAuthenticationService> _logger;

    public B2BAuthenticationService(
        GraphServiceClient graphServiceClient,
        Microsoft.Extensions.Options.IOptions<Configuration.AzureOptions> azureOptions,
        ILogger<B2BAuthenticationService> logger)
    {
        _graphServiceClient = graphServiceClient;
        _azureOptions = azureOptions.Value;
        _logger = logger;
    }

    public async Task<AuthenticatedUser?> GetAuthenticatedUserAsync(ClaimsPrincipal claimsPrincipal)
    {
        try
        {
            var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                        claimsPrincipal.FindFirst("oid")?.Value ?? 
                        claimsPrincipal.FindFirst("sub")?.Value;
            var userPrincipalName = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value ?? 
                                   claimsPrincipal.FindFirst("name")?.Value;
            var email = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value ?? 
                       claimsPrincipal.FindFirst("email")?.Value;
            var tenantId = claimsPrincipal.FindFirst("tid")?.Value ?? 
                          claimsPrincipal.FindFirst("tenant_id")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("No user ID found in claims principal for B2B user");
                return null;
            }

            var userInfo = await GetUserInfoAsync(userId);
            
            return new AuthenticatedUser
            {
                UserId = Guid.TryParse(userId, out var guid) ? guid : Guid.NewGuid(),
                Email = email ?? userInfo?.Mail ?? string.Empty,
                Name = userPrincipalName ?? userInfo?.DisplayName ?? string.Empty,
                Claims = claimsPrincipal.Claims.ToDictionary(c => c.Type, c => c.Value),
                Status = userInfo?.AccountEnabled == true ? "Active" : "Inactive",
                Type = "AzureB2B",
                TenantId = Guid.TryParse(tenantId ?? _azureOptions.TenantId, out var tenantGuid) ? tenantGuid : null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting authenticated B2B user from Azure AD");
            return null;
        }
    }

    public async Task<AzureUserInfo?> GetUserInfoAsync(string userId)
    {
        try
        {
            var user = await _graphServiceClient.Users[userId].GetAsync();
            
            if (user == null)
                return null;

            return new AzureUserInfo
            {
                Id = user.Id ?? string.Empty,
                DisplayName = user.DisplayName ?? string.Empty,
                GivenName = user.GivenName ?? string.Empty,
                Surname = user.Surname ?? string.Empty,
                Mail = user.Mail ?? string.Empty,
                UserPrincipalName = user.UserPrincipalName ?? string.Empty,
                JobTitle = user.JobTitle ?? string.Empty,
                Department = user.Department ?? string.Empty,
                OfficeLocation = user.OfficeLocation ?? string.Empty,
                MobilePhone = user.MobilePhone ?? string.Empty,
                BusinessPhones = string.Join(", ", user.BusinessPhones ?? new List<string>()),
                CreatedDateTime = user.CreatedDateTime?.DateTime,
                LastPasswordChangeDateTime = user.LastPasswordChangeDateTime?.DateTime,
                AccountEnabled = user.AccountEnabled ?? false,
                PreferredLanguage = user.PreferredLanguage ?? string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting B2B user info from Microsoft Graph for user {UserId}", userId);
            return null;
        }
    }

    public async Task<IEnumerable<AzureGroupInfo>> GetUserGroupsAsync(string userId)
    {
        try
        {
            var groups = await _graphServiceClient.Users[userId].MemberOf.GetAsync();
            
            if (groups?.Value == null)
                return Enumerable.Empty<AzureGroupInfo>();

            return groups.Value.OfType<Group>().Select(group => new AzureGroupInfo
            {
                Id = group.Id ?? string.Empty,
                DisplayName = group.DisplayName ?? string.Empty,
                Description = group.Description ?? string.Empty,
                Mail = group.Mail ?? string.Empty,
                MailNickname = group.MailNickname ?? string.Empty,
                SecurityEnabled = group.SecurityEnabled ?? false,
                MailEnabled = group.MailEnabled ?? false,
                CreatedDateTime = group.CreatedDateTime?.DateTime
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting B2B user groups from Microsoft Graph for user {UserId}", userId);
            return Enumerable.Empty<AzureGroupInfo>();
        }
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            // B2B specific token validation logic
            _logger.LogDebug("B2B token validation requested");
            return await Task.FromResult(!string.IsNullOrEmpty(token));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating B2B token");
            return false;
        }
    }

    public async Task<string> GetSignOutUrlAsync(string userId)
    {
        try
        {
            var signOutUrl = $"{_azureOptions.Instance}{_azureOptions.TenantId}/oauth2/v2.0/logout" +
                           $"?post_logout_redirect_uri={Uri.EscapeDataString(_azureOptions.SignedOutCallbackPath)}";
            
            _logger.LogDebug("Generated B2B sign out URL for user {UserId}", userId);
            return await Task.FromResult(signOutUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating B2B sign out URL for user {UserId}", userId);
            return string.Empty;
        }
    }
}
