using Microsoft.Identity.Client;

namespace Mamey.Graph.Msal;

public class MsalClientBuilder
{
    
    public class PublicClient
    {
        private readonly IPublicClientApplication _publicClientApplication;
        private readonly MameyPublicClientOptions _publicClientOptions;
        public PublicClient(MameyPublicClientOptions publicClientOptions)
        {
            _publicClientOptions = publicClientOptions;
        }
        public IPublicClientApplication BuildPublicClient(PublicClientApplicationBuilder buidler)
        {
            return PublicClientApplicationBuilder.Create(_publicClientOptions.ClientId)
                                                 .WithTenantId(_publicClientOptions.TenantId)
                                                 .WithRedirectUri(_publicClientOptions.RedirectUri)
                                                 .WithB2CAuthority(_publicClientOptions.Authority)
                                                 .Build();
        }

        public async Task<AuthenticationResult> AquireTokenSilentlyAsync()
        {
            if (_publicClientApplication is null)
            {
                throw new NullReferenceException($"{nameof(_publicClientApplication)} cannot be null.");
            }
            var accounts = await _publicClientApplication.GetAccountsAsync();

            string[] scopes = new string[] { "user.read" };
            AuthenticationResult? result = null;

            try
            {

                result = await _publicClientApplication
                    .AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                    .ExecuteAsync();
            }
            catch (MsalUiRequiredException)
            {
                try
                {
                    result = await _publicClientApplication.AcquireTokenInteractive(scopes).ExecuteAsync();

                }
                catch (MsalException msalex)
                {
                    var message = $"Error Acquiring Token:{System.Environment.NewLine}{msalex}";

                }
            }
            catch(Exception ex)
            {
                
                var message = $"Error Acquiring Token Silently:{System.Environment.NewLine}{ex}";
  
            }

            if (result != null)
            {
                string accessToken = result.AccessToken;
                // Use the token
            }
            return result;
        }
        public async Task ClearTokenCacheAsync()
        {
            var accounts = (await _publicClientApplication.GetAccountsAsync()).ToList();

            // clear the cache
            while (accounts.Any())
            {
                await _publicClientApplication.RemoveAsync(accounts.First());
                accounts = (await _publicClientApplication.GetAccountsAsync()).ToList();
            }
        }
    }
    public class ConfidentialClient
    {
        private readonly IConfidentialClientApplication _confidentialClientApplication;

        public ConfidentialClient(IConfidentialClientApplication confidentialClientApplication,
            MameyConfidentialClientOptions confidentialClientOptions)
        {
            if (confidentialClientApplication is null)
            {
                throw new NullReferenceException($"{nameof(confidentialClientApplication)} cannot be null.");
            }
            _confidentialClientApplication = ConfidentialClientApplicationBuilder.Create(confidentialClientOptions.ClientId)
                                                       .WithClientSecret(confidentialClientOptions.ClientSecret)
                                                       .WithAuthority(confidentialClientOptions.Authority)
                                                       .Build();
            TokenCacheHelper.EnableSerialization(confidentialClientApplication.UserTokenCache);
        }
        //public async Task<AuthenticationResult> AquireTokenOnBehalfOf()
        //{
        //    AuthenticationResult result;
        //    try
        //    {
                
        //        result = await _confidentialClientApplication.AcquireTokenOnBehalfOf(scopes,);
        //    }
        //    catch (MsalException ex)
        //    {

        //    }
        //}
        public async Task<AuthenticationResult> AquireTokenForClientAsync()
        {
            string[] scopes = new string[] { "user.read" };
            AuthenticationResult result;
            try
            {
                result = await _confidentialClientApplication.AcquireTokenForClient(scopes: new[] { "some_app_id_uri/.default" })        // Uses the token cache automatically, which is optimized for multi-tenant access
                .WithAuthority(AzureCloudInstance.AzurePublic, "{tenantID}")  // Do not use "common" or "organizations"!
                .ExecuteAsync();
                return result;
            }
            catch (MsalException)
            {
                throw;
            }
        }
    }
}
