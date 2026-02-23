using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Extensions.Logging;

namespace Mamey.Identity.Azure.Graph;

/// <summary>
/// Microsoft Graph service implementation.
/// </summary>
public class GraphService : IGraphService
{
    private readonly GraphServiceClient _graphServiceClient;
    private readonly ILogger<GraphService> _logger;

    public GraphService(GraphServiceClient graphServiceClient, ILogger<GraphService> logger)
    {
        _graphServiceClient = graphServiceClient;
        _logger = logger;
    }

    public GraphServiceClient Client => _graphServiceClient;

    public async Task<User?> GetUserAsync(string userId)
    {
        try
        {
            return await _graphServiceClient.Users[userId].GetAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user {UserId} from Microsoft Graph", userId);
            return null;
        }
    }

    public async Task<User?> GetUserByPrincipalNameAsync(string userPrincipalName)
    {
        try
        {
            var users = await _graphServiceClient.Users.GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Filter = $"userPrincipalName eq '{userPrincipalName}'";
            });

            return users?.Value?.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by principal name {UserPrincipalName} from Microsoft Graph", userPrincipalName);
            return null;
        }
    }

    public async Task<IEnumerable<Group>> GetUserGroupsAsync(string userId)
    {
        try
        {
            var groups = await _graphServiceClient.Users[userId].MemberOf.GetAsync();
            return groups?.Value?.OfType<Group>() ?? Enumerable.Empty<Group>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting groups for user {UserId} from Microsoft Graph", userId);
            return Enumerable.Empty<Group>();
        }
    }

    public async Task<IEnumerable<User>> GetUsersAsync(int top = 100, int skip = 0)
    {
        try
        {
            var users = await _graphServiceClient.Users.GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Top = top;
            });

            return users?.Value ?? Enumerable.Empty<User>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting users from Microsoft Graph");
            return Enumerable.Empty<User>();
        }
    }

    public async Task<IEnumerable<Group>> GetGroupsAsync(int top = 100, int skip = 0)
    {
        try
        {
            var groups = await _graphServiceClient.Groups.GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Top = top;
            });

            return groups?.Value ?? Enumerable.Empty<Group>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting groups from Microsoft Graph");
            return Enumerable.Empty<Group>();
        }
    }

    public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm)
    {
        try
        {
            var users = await _graphServiceClient.Users.GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Search = $"\"{searchTerm}\"";
            });

            return users?.Value ?? Enumerable.Empty<User>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users with term {SearchTerm} from Microsoft Graph", searchTerm);
            return Enumerable.Empty<User>();
        }
    }

    public async Task<IEnumerable<Group>> SearchGroupsAsync(string searchTerm)
    {
        try
        {
            var groups = await _graphServiceClient.Groups.GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Search = $"\"{searchTerm}\"";
            });

            return groups?.Value ?? Enumerable.Empty<Group>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching groups with term {SearchTerm} from Microsoft Graph", searchTerm);
            return Enumerable.Empty<Group>();
        }
    }
}
