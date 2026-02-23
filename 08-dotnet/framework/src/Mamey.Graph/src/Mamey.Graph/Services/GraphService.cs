using Mamey.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;

namespace Mamey.Graph;

public class GraphService : IGraphService
{
    private readonly ILogger<GraphService> _logger;
    private readonly GraphOptions _options;
    private readonly GraphServiceClient _graphClient;

    public GraphService(ILogger<GraphService> logger, GraphServiceClient graphClient, GraphOptions options)
    {
        _logger = logger;
        _options = options;
        _ = _options ??
            throw new System.NullReferenceException("Settings cannot be null");
        _graphClient = graphClient;
        GraphHelper.InitializeGraphForAppOnlyAuth(options);
        
        
    }
    public async Task DisplayAccessTokenAsync()
    {
        try
        {
            var appOnlyToken = await GraphHelper.GetAppOnlyTokenAsync();
            Console.WriteLine($"App-only token: {appOnlyToken}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting app-only access token: {ex.Message}");
        }
    }
    public async Task MakeGraphCallAsync()
    {
        await GraphHelper.MakeGraphCallAsync();
    }
    public async Task<List<User>?> ListUsersAsync()
    {
        try
        {
            var userPage = await GraphHelper.GetUsersAsync();

            if (userPage?.Value == null)
            {
                Console.WriteLine("No results returned.");
                return null;
            }
            return userPage?.Value;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting users: {ex.Message}");
        }
        return null;
    }

    public Task<List<Message>> GetSharedMailboxMessagesAsync(string sharedMailboxEmail)
        => string.IsNullOrEmpty(sharedMailboxEmail)
        ? throw new ArgumentException($"'{nameof(sharedMailboxEmail)}' cannot be null or empty.",
            nameof(sharedMailboxEmail))
        : GraphHelper.GetSharedMailboxMessagesAsync(sharedMailboxEmail);

    public Task<Message?> SendSharedMailboxMessageAsync(string sharedMailboxEmail, Message message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(sharedMailboxEmail))
        {
            throw new ArgumentException($"'{nameof(sharedMailboxEmail)}' cannot be null or empty.", nameof(sharedMailboxEmail));
        }

        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }
        return GraphHelper.SendSharedMailboxMessageAsync(sharedMailboxEmail, message);
    }

    #region New
    public async Task<List<User>?> GetAllUsers()
    {
        _logger.LogInformation("Getting list of Users.");
        var result = (await _graphClient.Users.GetAsync())?.Value;
        return result;
    }
    public async Task<User?> GetUserByObjectId(Guid userId)
    {
        _logger.LogInformation("Getting list of Users.");
        var result = await _graphClient.Users[userId.ToString()].GetAsync();
        return result;
    }
    public Task GetUserBySignInName(string username)
    {
        throw new NotImplementedException();
    }
    public Task DeleteUserByObjectId(Guid objectId)
    {
        throw new NotImplementedException();
    }
    public Task UpdateUserPassword()
    {
        throw new NotImplementedException();
    }
    public async  Task<List<User>>? CreateUsers(List<User> users)
    {
        var newUsers = new List<User>();

        users.ForEach(async u => {
            var newUser = await CreateUserAsync(u);
            newUsers.Add(newUser);
        });

        return newUsers;
    }
    public async Task<User?> CreateUserAsync(User user)
    {
        if (user is null)
        {
            throw new ArgumentNullException(nameof(user));
        }

        try
        {

            return await _graphClient.Users.PostAsync(user);
        }
        catch (ODataError ex)
        {
            throw new MameyException($"CreateUser Exception for {user.UserPrincipalName}: {ex.Error.Message}", ex.Error.Code, ex.Message);
        }
    }
    #endregion
}