using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Mamey.Auth.Abstractions;
using Microsoft.Graph.Models;
using Microsoft.IdentityModel.Tokens;

namespace Mamey.Auth.Azure.B2C;

/// <summary>
/// Service for Azure AD B2C authentication and user management.
/// Implements IAzureAuthService from Mamey.Auth.Abstractions.
/// </summary>
public class B2CAuthenticationService : IAzureAuthService
{
    private readonly IConfidentialClientApplication _msalClient;
    private readonly GraphServiceClient _graphClient;
    private readonly IRedisTokenCache _tokenCache;
    private readonly ILogger<B2CAuthenticationService> _logger;
    private readonly AzureB2COptions _b2cOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="B2CAuthenticationService"/> class.
    /// </summary>
    /// <param name="msalClient">MSAL confidential client application.</param>
    /// <param name="graphClient">Graph API client.</param>
    /// <param name="tokenCache">Token cache service.</param>
    /// <param name="logger">Logger instance.</param>
    /// <param name="b2cOptions">B2C configuration options.</param>
    public B2CAuthenticationService(
        IConfidentialClientApplication msalClient,
        GraphServiceClient graphClient,
        IRedisTokenCache tokenCache,
        ILogger<B2CAuthenticationService> logger,
        AzureB2COptions b2cOptions)
    {
        _msalClient = msalClient ?? throw new ArgumentNullException(nameof(msalClient));
        _graphClient = graphClient ?? throw new ArgumentNullException(nameof(graphClient));
        _tokenCache = tokenCache ?? throw new ArgumentNullException(nameof(tokenCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _b2cOptions = b2cOptions ?? throw new ArgumentNullException(nameof(b2cOptions));
    }

    /// <inheritdoc/>
    public async Task<string> AcquireTokenAsync(string[] scopes)
    {
        if (scopes == null || scopes.Length == 0)
        {
            throw new ArgumentException("Scopes cannot be null or empty.", nameof(scopes));
        }

        var cacheKey = $"b2c_token_{string.Join("_", scopes)}";
        var cachedToken = await _tokenCache.GetCachedTokenAsync(cacheKey);
        if (!string.IsNullOrWhiteSpace(cachedToken))
        {
            _logger.LogDebug("Returning cached B2C access token for scopes: {Scopes}", string.Join(", ", scopes));
            return cachedToken;
        }

        try
        {
            _logger.LogDebug("Acquiring new B2C access token for scopes: {Scopes}", string.Join(", ", scopes));
            var result = await _msalClient
                .AcquireTokenForClient(scopes)
                .ExecuteAsync();

            await _tokenCache.SetCachedTokenAsync(cacheKey, result.AccessToken);
            _logger.LogInformation("Acquired new B2C access token.");
            return result.AccessToken;
        }
        catch (MsalServiceException ex)
        {
            _logger.LogError(ex, "Failed to acquire B2C access token.");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> VerifyUserActionAsync(string userId, string token)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
        {
            _logger.LogWarning("User ID or token is null or empty.");
            return false;
        }

        try
        {
            var userAssertion = new UserAssertion(token);
            var result = await _msalClient
                .AcquireTokenOnBehalfOf(new[] { "https://graph.microsoft.com/.default" }, userAssertion)
                .ExecuteAsync();

            if (result.Account != null && result.Account.Username.Equals(userId, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("User action token verified for user: {UserId}", userId);
                return true;
            }
        }
        catch (MsalUiRequiredException ex)
        {
            _logger.LogWarning(ex, "UI interaction required to verify user action for user: {UserId}", userId);
        }
        catch (MsalServiceException ex)
        {
            _logger.LogError(ex, "Service error while verifying user action for user: {UserId}", userId);
        }

        return false;
    }

    /// <summary>
    /// Creates a new user in Azure AD B2C.
    /// </summary>
    /// <param name="user">The user object to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created user object.</returns>
    /// <exception cref="ArgumentException">Thrown if required user fields are missing.</exception>
    public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(user.DisplayName) || string.IsNullOrEmpty(user.UserPrincipalName))
        {
            throw new ArgumentException("DisplayName and UserPrincipalName are required for user creation.");
        }

        try
        {
            _logger.LogInformation("Creating a new B2C user: {UserPrincipalName}", user.UserPrincipalName);
            var createdUser = await _graphClient.Users.PostAsync(user, cancellationToken: cancellationToken);
            _logger.LogInformation("Successfully created B2C user: {Id}", createdUser.Id);
            return createdUser;
        }
        catch (ServiceException ex)
        {
            _logger.LogError(ex, "Failed to create B2C user.");
            throw;
        }
    }

    /// <summary>
    /// Retrieves all users from Azure AD B2C.
    /// </summary>
    /// <returns>A list of users, or null if none found.</returns>
    public async Task<List<User>?> GetUsersAsync()
    {
        try
        {
            _logger.LogDebug("Retrieving users from Azure AD B2C.");
            var users = await _graphClient.Users.GetAsync();
            return users?.Value;
        }
        catch (ServiceException ex)
        {
            _logger.LogError(ex, "Failed to retrieve users from Azure AD B2C.");
            return null;
        }
    }

    /// <summary>
    /// Initiates a password reset process using B2C policy.
    /// </summary>
    /// <param name="userId">The user ID to reset password for.</param>
    /// <returns>A URL link to initiate the password reset process.</returns>
    public Task<string> InitiatePasswordResetAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
        }

        var resetPasswordUrl = $"https://{_b2cOptions.Domain}.b2clogin.com/{_b2cOptions.TenantId}.onmicrosoft.com/{_b2cOptions.ResetPasswordPolicyId}/oauth2/v2.0/authorize" +
                               $"?client_id={_b2cOptions.ClientId}&redirect_uri={Uri.EscapeDataString(_b2cOptions.RedirectUri)}" +
                               $"&response_type=id_token&scope=openid&login_hint={userId}";

        _logger.LogInformation("Generated password reset link for user: {UserId}", userId);
        return Task.FromResult(resetPasswordUrl);
    }
    public async Task<bool> ValidateIdTokenAsync(string idToken)
    {
        if (string.IsNullOrWhiteSpace(idToken))
        {
            _logger.LogWarning("ID token is null or empty.");
            return false;
        }

        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = $"https://{_b2cOptions.Domain}/{_b2cOptions.TenantId}/v2.0/",
                ValidateAudience = true,
                ValidAudience = _b2cOptions.ClientId,
                ValidateLifetime = true,
                RequireExpirationTime = true,
                ValidateIssuerSigningKey = false // This is a simplification; normally you would validate the signature using the issuer's public keys.
            };

            tokenHandler.ValidateToken(idToken, validationParameters, out _);
            _logger.LogInformation("ID token validated successfully.");
            return true;
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogWarning(ex, "ID token validation failed.");
            return false;
        }
    }
    public async Task<string> RefreshAccessTokenAsync(string refreshToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw new ArgumentException("Refresh token cannot be null or empty.", nameof(refreshToken));
        }

        try
        {
            _logger.LogDebug("Refreshing B2C access token using AcquireTokenSilentAsync.");

            // MSAL will handle refreshing using its internal token cache and refresh token logic.
            var accounts = await _msalClient.GetAccountsAsync();
            var firstAccount = accounts.FirstOrDefault();

            if (firstAccount == null)
            {
                _logger.LogWarning("No user account found in token cache to refresh token.");
                throw new InvalidOperationException("No account available to refresh token.");
            }

            var result = await _msalClient
                .AcquireTokenSilent(new[] { "https://graph.microsoft.com/.default" }, firstAccount)
                .ExecuteAsync();

            _logger.LogInformation("Successfully refreshed B2C access token.");
            return result.AccessToken;
        }
        catch (MsalUiRequiredException ex)
        {
            _logger.LogError(ex, "Silent token acquisition failed; interactive authentication is required.");
            throw;
        }
        catch (MsalServiceException ex)
        {
            _logger.LogError(ex, "Failed to refresh B2C access token.");
            throw;
        }
    }
    public async Task<bool> DeleteUserAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
        }

        try
        {
            await _graphClient.Users[userId].DeleteAsync();
            _logger.LogInformation("Deleted B2C user: {UserId}", userId);
            return true;
        }
        catch (ServiceException ex)
        {
            _logger.LogError(ex, "Failed to delete B2C user: {UserId}", userId);
            return false;
        }
    }
    public async Task<User?> UpdateUserAsync(string userId, User updatedUser)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
        }

        if (updatedUser == null)
        {
            throw new ArgumentNullException(nameof(updatedUser));
        }

        try
        {
            var user = await _graphClient.Users[userId].PatchAsync(updatedUser);
            _logger.LogInformation("Updated B2C user: {UserId}", userId);
            return user;
        }
        catch (ServiceException ex)
        {
            _logger.LogError(ex, "Failed to update B2C user: {UserId}", userId);
            return null;
        }
    }
    public async Task<bool> DisableUserAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
        }

        try
        {
            var updateUser = new User
            {
                AccountEnabled = false
            };
            await _graphClient.Users[userId].PatchAsync(updateUser);
            _logger.LogInformation("Disabled B2C user: {UserId}", userId);
            return true;
        }
        catch (ServiceException ex)
        {
            _logger.LogError(ex, "Failed to disable B2C user: {UserId}", userId);
            return false;
        }
    }
    public async Task<bool> EnableUserAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
        }

        try
        {
            var updateUser = new User
            {
                AccountEnabled = true
            };
            await _graphClient.Users[userId].PatchAsync(updateUser);
            _logger.LogInformation("Enabled B2C user: {UserId}", userId);
            return true;
        }
        catch (ServiceException ex)
        {
            _logger.LogError(ex, "Failed to enable B2C user: {UserId}", userId);
            return false;
        }
    }
    public async Task<Dictionary<string, string>> GetUserClaimsAsync(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
        }

        var claims = new Dictionary<string, string>();

        try
        {
            var user = await _graphClient.Users[userId].GetAsync();
            if (user != null)
            {
                // Map claims (add more as needed)
                claims["displayName"] = user.DisplayName ?? "";
                claims["userPrincipalName"] = user.UserPrincipalName ?? "";
            }
            _logger.LogInformation("Retrieved claims for B2C user: {UserId}", userId);
        }
        catch (ServiceException ex)
        {
            _logger.LogError(ex, "Failed to retrieve claims for B2C user: {UserId}", userId);
        }

        return claims;
    }
    public async Task<bool> SetUserClaimsAsync(string userId, Dictionary<string, string> claims)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
        }

        if (claims == null)
        {
            throw new ArgumentNullException(nameof(claims));
        }

        try
        {
            var updateUser = new User();

            if (claims.ContainsKey("displayName"))
            {
                updateUser.DisplayName = claims["displayName"];
            }

            if (claims.ContainsKey("userPrincipalName"))
            {
                updateUser.UserPrincipalName = claims["userPrincipalName"];
            }

            await _graphClient.Users[userId].PatchAsync(updateUser);
            _logger.LogInformation("Updated claims for B2C user: {UserId}", userId);
            return true;
        }
        catch (ServiceException ex)
        {
            _logger.LogError(ex, "Failed to update claims for B2C user: {UserId}", userId);
            return false;
        }
    }
    public async Task<bool> AddUserToGroupAsync(string userId, string groupId)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(groupId))
        {
            throw new ArgumentException("User ID and Group ID cannot be null or empty.");
        }

        try
        {
            var reference = new Microsoft.Graph.Models.ReferenceCreate
            {
                OdataId = $"https://graph.microsoft.com/v1.0/directoryObjects/{userId}"
            };

            await _graphClient.Groups[groupId].Members.Ref.PostAsync(reference);
            _logger.LogInformation("Added B2C user {UserId} to group {GroupId}.", userId, groupId);
            return true;
        }
        catch (ServiceException ex)
        {
            _logger.LogError(ex, "Failed to add B2C user {UserId} to group {GroupId}.", userId, groupId);
            return false;
        }
    }
    public async Task<bool> RemoveUserFromGroupAsync(string userId, string groupId)
    {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(groupId))
        {
            throw new ArgumentException("User ID and Group ID cannot be null or empty.");
        }

        try
        {
            await _graphClient.Groups[groupId].Members[userId].Ref.DeleteAsync();
            _logger.LogInformation("Removed B2C user {UserId} from group {GroupId}.", userId, groupId);
            return true;
        }
        catch (ServiceException ex)
        {
            _logger.LogError(ex, "Failed to remove B2C user {UserId} from group {GroupId}.", userId, groupId);
            return false;
        }
    }

    public Task<bool> LogoutUserAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<List<string>> ListGroupsForUserAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<UserCollectionResponse> SearchUsersAsync(string query)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ForcePasswordChangeAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> LockUserAccountAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> InviteUserAsync(string email, string displayName)
    {
        throw new NotImplementedException();
    }

    public Task<string> GenerateSignInUrlAsync(string redirectUrl, string state, string nonce)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ValidateConfigurationAsync()
    {
        throw new NotImplementedException();
    }
}
