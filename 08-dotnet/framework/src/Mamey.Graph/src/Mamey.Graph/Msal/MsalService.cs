using Microsoft.Identity.Client;

namespace Mamey.Graph.Msal;

public class MsalService
{
    private readonly IPublicClientApplication _publicClient;
    private readonly IConfidentialClientApplication _confidentialClient;

    public MsalService(MameyPublicClientOptions publicOptions, MameyConfidentialClientOptions confidentialOptions)
    {
        _publicClient = PublicClientApplicationBuilder.Create(publicOptions.ClientId)
                                                      .WithTenantId(publicOptions.TenantId)
                                                      .WithRedirectUri(publicOptions.RedirectUri)
                                                      .WithB2CAuthority(publicOptions.Authority)
                                                      .Build();

        _confidentialClient = ConfidentialClientApplicationBuilder.Create(confidentialOptions.ClientId)
                                                                  .WithClientSecret(confidentialOptions.ClientSecret)
                                                                  .WithAuthority(confidentialOptions.Authority)
                                                                  .Build();
    }

    // Acquire token silently with possible exception handling for claims challenges
    public async Task<string> AcquireTokenSilentlyAsync(string[] scopes)
    {
        var accounts = await _publicClient.GetAccountsAsync();
        try
        {
            var result = await _publicClient.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                                            .ExecuteAsync();
            return result.AccessToken;
        }
        catch (MsalUiRequiredException ex)
        {
            // Optional: Handle claims challenges or other exceptions as necessary
            return await HandleExceptionAndAcquireTokenAsync(ex, scopes);
        }
        catch(MsalException)
        {
            throw;
        }
    }

    // Interactive token acquisition
    public async Task<string> AcquireTokenInteractiveAsync(string[] scopes)
    {
        try
        {
            var result = await _publicClient.AcquireTokenInteractive(scopes).ExecuteAsync();
            return result.AccessToken;
        }
        catch (MsalException ex)
        {
            // Log or handle exceptions accordingly
            throw new InvalidOperationException("Failed to acquire token interactively.", ex);
        }
    }

    // Acquire token for confidential client
    public async Task<string> AcquireTokenForClientAsync(string[] scopes)
    {
        try
        {
 
            var result = await _confidentialClient.AcquireTokenForClient(scopes).ExecuteAsync();
            return result.AccessToken;
        }
        catch (MsalException ex)
        {
            // Log or handle specific exceptions as necessary
            throw new InvalidOperationException("Failed to acquire token for client.", ex);
        }
    }

    // Get a list of all accounts known to the application
    public async Task<IEnumerable<Microsoft.Identity.Client.IAccount>> GetAccountsAsync()
    {
        return await _publicClient.GetAccountsAsync();
    }

    // Sign out a specific account
    public async Task RemoveAccountAsync(Microsoft.Identity.Client.IAccount account)
    {
        await _publicClient.RemoveAsync(account);
    }

    // Private method to handle exceptions and retry acquisition if necessary
    private async Task<string> HandleExceptionAndAcquireTokenAsync(MsalUiRequiredException ex, string[] scopes)
    {
        if (ex.Classification == UiRequiredExceptionClassification.ConsentRequired)
        {
            // Handle consent-specific logic or re-prompt for consent
            var interactiveResult = await _publicClient.AcquireTokenInteractive(scopes).ExecuteAsync();
            return interactiveResult.AccessToken;
        }
        throw new InvalidOperationException("Unhandled MSAL UI required exception.", ex);
    }
}