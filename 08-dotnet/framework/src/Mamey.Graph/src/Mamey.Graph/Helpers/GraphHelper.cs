using Azure.Core;
using Azure.Identity;
using Mamey.Exceptions;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace Mamey.Graph;

public partial class GraphHelper
{
    private static GraphOptions? _settings;
    private static ClientSecretCredential? _clientSecretCredential;
    private static GraphServiceClient? _appClient;

    public static void InitializeGraphForAppOnlyAuth(GraphOptions settings)
    {
        _settings = settings;

        // Ensure settings isn't null
        _ = settings ??
            throw new System.NullReferenceException("Settings cannot be null");
        if (string.IsNullOrEmpty(settings.ClientId))
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Application Error: ClientId secret cannot be null");
            Console.ResetColor();
        }
        if (string.IsNullOrEmpty(settings.ClientSecret))
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("Application Error: ClientSecret cannot be null");
            Console.ResetColor();
        }

        _settings = settings;

        if (_clientSecretCredential == null)
        {
            _clientSecretCredential = new ClientSecretCredential(
                _settings.TenantId, _settings.ClientId, _settings.ClientSecret);
        }

        if (_appClient == null)
        {
            _appClient = new GraphServiceClient(_clientSecretCredential,
                // Use the default scope, which will request the scopes
                // configured on the app registration
                new[] { "https://graph.microsoft.com/.default" });
        }
    }

    public static async Task<string> GetAppOnlyTokenAsync()
    {
        // Ensure credential isn't null
        _ = _clientSecretCredential ??
            throw new System.NullReferenceException("Graph has not been initialized for app-only auth");

        // Request token with given scopes
        var context = new TokenRequestContext(new[] { "https://graph.microsoft.com/.default" });
        var response = await _clientSecretCredential.GetTokenAsync(context);
        return response.Token;
    }
    // </GetAppOnlyTokenSnippet>

    // <GetUsersSnippet>
    public static Task<UserCollectionResponse?> GetUsersAsync()
    {
        // Ensure client isn't null
        _ = _appClient ??
            throw new System.NullReferenceException("Graph has not been initialized for app-only auth");

        return _appClient.Users.GetAsync((config) =>
        {
            // Only request specific properties
            config.QueryParameters.Select = new[] { "displayName", "id", "mail" };
            // Get at most 25 results
            config.QueryParameters.Top = 25;
            // Sort by display name
            config.QueryParameters.Orderby = new[] { "displayName" };
        });
    }
    public static async Task<List<Message>> GetSharedMailboxMessagesAsync(string sharedMailboxEmail)
    {
        try
        {
            // Get the shared mailbox messages
            var messages = (await _appClient.Users[sharedMailboxEmail].Messages.GetAsync())
                .Value;

            ;

            var telexInboundMessages = messages?
                .Where(m => m.From?.EmailAddress?.Address?.ToLower() == "inbound.telex@telexbynet.co.uk");

            return telexInboundMessages is null ? new List<Message>() : telexInboundMessages.ToList();
        }
        catch (ServiceException ex)
        {
            Console.WriteLine($"Error getting shared mailbox messages: {ex.Message}");
            return new List<Message>();
        }
    }
    public static async Task<Message?> SendSharedMailboxMessageAsync(string sharedMailboxEmail,
        Message message, CancellationToken cancellationToken = default)
        => _appClient is null
        ? throw new MameyException($"{nameof(_appClient)} instance of object not found.")
        : await _appClient.Users[sharedMailboxEmail]
                .Messages.PostAsync(message, cancellationToken: cancellationToken);

    public static class B2CUserManagement
    {
        public static async Task<User?> CreateUser<T>(T user, CancellationToken cancellationToken = default)
            where T : User
        {
            if(_appClient is null)
            {
                throw new ArgumentNullException(nameof(_appClient));
            }

            var result = await _appClient.Users.PostAsync(user);
            return result;
        }
        public static async Task AddUserToGroupAsync(string userId, string groupId, CancellationToken cancellationToken = default)
        {
            if (_appClient is null)
            {
                throw new ArgumentNullException(nameof(_appClient));
            }

            var requestBody = new ReferenceCreate
            {
                OdataId = "https://graph.microsoft.com/v1.0/directoryObjects/{id}",
            };
            var directoryObject = new DirectoryObject { Id = userId };
            await _appClient.Groups[groupId].Members.Ref.PostAsync(requestBody, cancellationToken: cancellationToken);
        }
    }
#pragma warning disable CS1998
    // <MakeGraphCallSnippet>
    // This function serves as a playground for testing Graph snippets
    // or other code
    public async static Task MakeGraphCallAsync()
    {
        // INSERT YOUR CODE HERE
    }
}
