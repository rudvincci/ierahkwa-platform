using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using IAccessTokenProvider =
    Microsoft.AspNetCore.Components.WebAssembly.Authentication.IAccessTokenProvider;

namespace Mamey.Graph.Services;

public class TokenService
{
    private readonly IAccessTokenProvider _tokenProvider;
    private AccessToken _cachedToken;

    public TokenService(IAccessTokenProvider tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        if (_cachedToken != null && !_cachedToken.Expires.InThePast())
        {
            return _cachedToken.Value;
        }

        var tokenResult = await _tokenProvider.RequestAccessToken();
        if (tokenResult.TryGetToken(out var token))
        {
            _cachedToken = token;
            return token.Value;
        }
        else
        {
            throw new InvalidOperationException("Unable to obtain access token. Please check your authentication settings.");
        }
    }
    //public async Task<string> TryAcquireTokenSilently()
    //{
    //    var accounts = await _msalClient.GetAccountsAsync();
    //    try
    //    {

    //        var result = await _msalClient.AcquireTokenSilent(_scopes, accounts.FirstOrDefault())
    //                                      .ExecuteAsync();
    //    }
    //    catch (MsalUiRequiredException ex)
    //    {
    //        if (ex.Claims != null)
    //        {
    //            // Handle conditional access claims challenge
    //            await _msalClient.AcquireTokenInteractive(_scopes)
    //                             .WithClaims(ex.Claims)
    //                             .ExecuteAsync();
    //        }
    //    }
    //    return result.AccessToken;
    //}
    //public void ConfigureTokenCache(ITokenCache tokenCache)
    //{
    //    tokenCache.SetBeforeAccess(BeforeAccessNotification);
    //    tokenCache.SetAfterAccess(AfterAccessNotification);
    //}
    //private void BeforeAccessNotification(TokenCacheNotificationArgs args)
    //{
    //    // Load the cache data from a secure location
    //    args.TokenCache.DeserializeMsalV3(secureStorage.Load());
    //}

    //private void AfterAccessNotification(TokenCacheNotificationArgs args)
    //{
    //    // Persist changes to the cache data
    //    if (args.HasStateChanged)
    //    {
    //        secureStorage.Save(args.TokenCache.SerializeMsalV3());
    //    }
    //}
    //public async Task<IEnumerable<IAccount>> GetAccountsAsync()
    //{
    //    return await _msalClient.GetAccountsAsync();
    //}

    //public async Task RemoveAccountAsync(IAccount account)
    //{
    //    await _msalClient.RemoveAsync(account);
    //}
    //public void ConfigureHttpClient()
    //{
    //    _msalClient.ClientApplicationBase.AppConfig.HttpClientFactory = new CustomHttpClientFactory();
    //}
    //public async Task<string> AcquireTokenByIntegratedWindowsAuth(string username)
    //{
    //    var result = await _msalClient.AcquireTokenByIntegratedWindowsAuth(_scopes)
    //                                  .WithUsername(username)
    //                                  .ExecuteAsync();
    //    return result.AccessToken;
    //}
    //public void EnableBroker()
    //{
    //    _msalClient.WithBroker(true);
    //}
    //public async Task<string> AcquireTokenForPolicy(string policy)
    //{
    //    var authority = $"https://{yourTenant}.b2clogin.com/{yourTenant}.onmicrosoft.com/{policy}";
    //    var pca = PublicClientApplicationBuilder.Create(clientId)
    //                                            .WithB2CAuthority(authority)
    //                                            .Build();
    //    var result = await pca.AcquireTokenInteractive(_scopes).ExecuteAsync();
    //    return result.AccessToken;
    //}
    //public void AddEventHandlers()
    //{
    //    _msalClient.ClientApplicationBase.AddAcquireTokenStartedNotification(context =>
    //    {
    //        Log.Information("Token acquisition started");
    //    });

    //    _msalClient.ClientApplicationBase.AddAcquireTokenCompletedNotification(context =>
    //    {
    //Log.Information("Token acquisition completed");
    //    });
    //}
}
