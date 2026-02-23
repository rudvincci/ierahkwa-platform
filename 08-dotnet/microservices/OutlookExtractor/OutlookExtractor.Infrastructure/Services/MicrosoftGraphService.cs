using System;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using OutlookExtractor.Core.Interfaces;

namespace OutlookExtractor.Infrastructure.Services;

/// <summary>
/// Microsoft Graph Service Implementation
/// Handles authentication and connection to Microsoft 365/Office 365/Outlook
/// IERAHKWA Platform Integration
/// </summary>
public class MicrosoftGraphService : IMicrosoftGraphService
{
    private GraphServiceClient? _graphClient;
    private string _userEmail = string.Empty;
    private string _userDisplayName = string.Empty;
    private bool _isAuthenticated = false;

    public async Task<bool> AuthenticateAsync(string tenantId, string clientId, string clientSecret)
    {
        try
        {
            var clientSecretCredential = new ClientSecretCredential(
                tenantId,
                clientId,
                clientSecret
            );

            _graphClient = new GraphServiceClient(clientSecretCredential);

            // Test authentication by getting user info
            var user = await _graphClient.Me.GetAsync();
            if (user != null && user.Mail != null)
            {
                _userEmail = user.Mail;
                _userDisplayName = user.DisplayName ?? string.Empty;
                _isAuthenticated = true;
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Authentication failed: {ex.Message}");
            _isAuthenticated = false;
            return false;
        }
    }

    public async Task<bool> AuthenticateInteractiveAsync(string clientId)
    {
        try
        {
            var options = new InteractiveBrowserCredentialOptions
            {
                ClientId = clientId,
                TenantId = "common",
                RedirectUri = new Uri("http://localhost:5000/auth/callback")
            };

            var interactiveCredential = new InteractiveBrowserCredential(options);
            _graphClient = new GraphServiceClient(interactiveCredential);

            // Test authentication
            var user = await _graphClient.Me.GetAsync();
            if (user != null && user.Mail != null)
            {
                _userEmail = user.Mail;
                _userDisplayName = user.DisplayName ?? string.Empty;
                _isAuthenticated = true;
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Interactive authentication failed: {ex.Message}");
            _isAuthenticated = false;
            return false;
        }
    }

    public Task<bool> IsAuthenticatedAsync()
    {
        return Task.FromResult(_isAuthenticated);
    }

    public Task<string> GetUserEmailAsync()
    {
        return Task.FromResult(_userEmail);
    }

    public Task<string> GetUserDisplayNameAsync()
    {
        return Task.FromResult(_userDisplayName);
    }

    public GraphServiceClient GetGraphClient()
    {
        if (_graphClient == null || !_isAuthenticated)
        {
            throw new InvalidOperationException("Not authenticated. Please authenticate first.");
        }
        return _graphClient;
    }
}
