namespace Mamey.Auth.Azure.B2B;
using Microsoft.Graph.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
public interface IB2BAuthenticationService
{
    Task<string> AcquireTokenAsync(string[] scopes);
    Task<User?> CreateUserAsync(User user, CancellationToken cancellationToken = default);
    Task<List<User>?> GetUsersAsync();
    Task<bool> AssignRoleAsync(string userId, string role);
    Task<bool> VerifyUserActionAsync(string userId, string token);
}
public class B2BAuthenticationService : IB2BAuthenticationService
{
    private readonly IConfidentialClientApplication _msalClient;
    private readonly GraphServiceClient _graphClient;
    private readonly IRedisTokenCache _redisTokenCache;

    public B2BAuthenticationService(IConfidentialClientApplication msalClient, GraphServiceClient graphClient, IRedisTokenCache redisTokenCache)
    {
        _msalClient = msalClient;
        _graphClient = graphClient;
        _redisTokenCache = redisTokenCache;
    }

    public async Task<string> AcquireTokenAsync(string[] scopes)
    {
        var cacheKey = $"token_{string.Join("_", scopes)}";
        var cachedToken = await _redisTokenCache.GetCachedTokenAsync(cacheKey);

        if (cachedToken != null)
        {
            return cachedToken;
        }

        var result = await _msalClient.AcquireTokenForClient(scopes).ExecuteAsync();
        await _redisTokenCache.SetCachedTokenAsync(cacheKey, result.AccessToken);

        return result.AccessToken;
    }

    public async Task<User?> CreateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        return await _graphClient.Users.PostAsync(user, cancellationToken: cancellationToken);
    }

    public async Task<List<User>?> GetUsersAsync()
    {
        var users = await _graphClient.Users.GetAsync();
        return users?.Value;
    }

    public async Task<bool> AssignRoleAsync(string userId, string role)
    {
        // var directoryRole = await _graphClient.DirectoryRoles.
        //     .Filter($"displayName eq '{role}'")
        //     .GetAsync();

        // if (directoryRole.Count == 0)
        // {
        //     throw new Exception("Role not found.");
        // }

        // await _graphClient.DirectoryRoles[directoryRole[0].Id].Members.References
        //     .Request().AddAsync(new DirectoryObject { Id = userId });
        throw new NotImplementedException();
        return true;
    }

    public async Task<bool> VerifyUserActionAsync(string userId, string token)
    {
        // Add custom validation logic here, e.g., verifying a token via Microsoft Authenticator.
        return token == "expected_token";
    }
}