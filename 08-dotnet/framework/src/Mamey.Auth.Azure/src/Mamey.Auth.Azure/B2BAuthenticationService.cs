using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Identity.Client;

namespace Mamey.Auth.Azure;

public class B2BAuthenticationService : Mamey.Auth.Abstractions.IAzureAuthService
{
    private readonly IConfidentialClientApplication _msalClient;
    private readonly GraphServiceClient _graphClient;
    private readonly IRedisTokenCache _redisTokenCache;
    private readonly ILogger<B2BAuthenticationService> _logger;

    public AuthenticatedUser? AuthenticatedUser => throw new NotImplementedException();

    public bool IsAuthenticated => throw new NotImplementedException();

    public event Action<AuthenticatedUser?> AuthenticatedUserChanged;

    public B2BAuthenticationService(IConfidentialClientApplication msalClient, GraphServiceClient graphClient, IRedisTokenCache redisTokenCache, ILogger<B2BAuthenticationService> logger)
    {
        _msalClient = msalClient;
        _graphClient = graphClient;
        _redisTokenCache = redisTokenCache;
        _logger = logger;
    }

    public async Task<string> AcquireTokenAsync(string[] scopes)
    {
        var cacheKey = $"token_{string.Join("_", scopes)}";
        var cachedToken = await _redisTokenCache.GetCachedTokenAsync(cacheKey);

        if (cachedToken != null)
        {
            return cachedToken;
        }

        try
        {
            var result = await _msalClient.AcquireTokenForClient(scopes).ExecuteAsync();
            await _redisTokenCache.SetCachedTokenAsync(cacheKey, result.AccessToken);
            return result.AccessToken;
        }
        catch (MsalServiceException ex)
        {
            _logger.LogError(ex, "Error acquiring token.");
            throw;
        }
    }

    public Task<bool> VerifyUserActionAsync(string userId, string token)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ValidateIdTokenAsync(string idToken)
    {
        throw new NotImplementedException();
    }

    public Task<string> RefreshAccessTokenAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteUserAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<User?> UpdateUserAsync(string userId, User updatedUser)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DisableUserAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> EnableUserAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<Dictionary<string, string>> GetUserClaimsAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SetUserClaimsAsync(string userId, Dictionary<string, string> claims)
    {
        throw new NotImplementedException();
    }

    public Task<bool> AddUserToGroupAsync(string userId, string groupId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RemoveUserFromGroupAsync(string userId, string groupId)
    {
        throw new NotImplementedException();
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

    public async Task<User?> CreateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _graphClient.Users.PostAsync(user, cancellationToken: cancellationToken);
        }
        catch (ServiceException ex)
        {
            _logger.LogError(ex, "Error creating user.");
            throw;
        }
    }

    public async Task<List<User>?> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var users = await _graphClient.Users.GetAsync(cancellationToken: cancellationToken);
            return users?.Value;
        }
        catch (ServiceException ex)
        {
            _logger.LogError(ex, "Error retrieving users.");
            throw;
        }
    }

    //public async Task<bool> AssignRoleAsync(string userId, string role, CancellationToken cancellationToken = default)
    //{
    //    try
    //    {
    //        var directoryRoles = await _graphClient.DirectoryRoles
    //            .
    //            .Filter($"displayName eq '{role}'")
    //            .GetAsync(cancellationToken);

    //        if (directoryRoles.CurrentPage.Count == 0)
    //        {
    //            _logger.LogError("Role not found.");
    //            return false;
    //        }

    //        var roleId = directoryRoles.CurrentPage[0].Id;
    //        var directoryObject = new DirectoryObject { Id = userId };

    //        await _graphClient.DirectoryRoles[roleId].Members.References
    //            .Request()
    //            .AddAsync(directoryObject, cancellationToken);

    //        return true;
    //    }
    //    catch (ServiceException ex)
    //    {
    //        _logger.LogError(ex, "Error assigning role.");
    //        throw;
    //    }
    //}

    public async Task<string> AcquireTokenForApplicationAsync(string[] scopes, string clientId)
    {
        var cacheKey = $"token_{clientId}_{string.Join("_", scopes)}";
        var cachedToken = await _redisTokenCache.GetCachedTokenAsync(cacheKey);

        if (cachedToken != null)
        {
            return cachedToken;
        }

        try
        {
            var result = await _msalClient.AcquireTokenForClient(scopes)
                //.WithClientId(clientId)
                .ExecuteAsync();

            await _redisTokenCache.SetCachedTokenAsync(cacheKey, result.AccessToken);

            return result.AccessToken;
        }
        catch (MsalServiceException ex)
        {
            _logger.LogError(ex, "Error acquiring token for application.");
            throw;
        }
    }

    public async Task<User?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _graphClient.Users[userId].GetAsync(cancellationToken: cancellationToken);
        }
        catch (ServiceException ex)
        {
            _logger.LogError(ex, "Error retrieving user by ID.");
            throw;
        }
    }

    public async Task<bool> DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            await _graphClient.Users[userId].DeleteAsync(cancellationToken: cancellationToken);
            return true;
        }
        catch (ServiceException ex)
        {
            _logger.LogError(ex, "Error deleting user.");
            return false;
        }
    }

    public async Task<Group?> CreateGroupAsync(Group group, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _graphClient.Groups.PostAsync(group, cancellationToken: cancellationToken);
        }
        catch (ServiceException ex)
        {
            _logger.LogError(ex, "Error creating group.");
            throw;
        }
    }

    public ClaimsPrincipal Authenticate(string credentials)
    {
        throw new NotImplementedException();
    }

    public bool Authorize(ClaimsPrincipal user, string resource)
    {
        throw new NotImplementedException();
    }

    public Task InitializeAsync()
    {
        throw new NotImplementedException();
    }

    public Task SignIn(string username, string password)
    {
        throw new NotImplementedException();
    }

    public void Logout()
    {
        throw new NotImplementedException();
    }
}