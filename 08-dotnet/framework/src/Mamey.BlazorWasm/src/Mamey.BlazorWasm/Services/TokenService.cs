    //using Mamey.BlazorWasm.Http;
    //using Mamey.BlazorWasm.Providers;
    //using Microsoft.AspNetCore.Components.Authorization;
    //using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
    //using static System.Formats.Asn1.AsnWriter;

    //namespace Mamey.BlazorWasm;

    //public class TokenService : ITokenService
    //{
    //    private readonly ILocalStorageService _localStorageService;
    //    //private readonly TokenAuthenticationStateProvider _authenticationStateProvider;
    //    private readonly AppState _appState;

    //    private const string AccessTokenKey = "accessToken";
    //    private const string RefreshTokenKey = "refreshToken";

    //    public TokenService(ILocalStorageService localStorageService, AppState appState)
    //    {
    //        _localStorageService = localStorageService;
    //       // _authenticationStateProvider = (TokenAuthenticationStateProvider)authenticationStateProvider;
    //        _appState = appState;
    //        _appState.OnAccessTokenChange += RefreshTokenIfNeeded;
    //    }

    //    public async Task<string> GetAccessTokenAsync()
    //    {
    //        return await _localStorageService.GetItemAsync<string>(AccessTokenKey);
    //    }

    //    public async Task<string> GetRefreshTokenAsync()
    //    {
    //        return await _localStorageService.GetItemAsync<string>(RefreshTokenKey);
    //    }

    //    public async Task StoreAccessTokenAsync(string accessToken)
    //    {
    //        if (string.IsNullOrEmpty(accessToken))
    //            throw new ArgumentException("Invalid token. Token cannot be null or empty.");

    //        await _localStorageService.SetItemAsync(AccessTokenKey, accessToken);
    //        _appState.AccessToken = accessToken;
    //    }

    //    public async Task StoreRefreshTokenAsync(string refreshToken)
    //    {
    //        if (string.IsNullOrEmpty(refreshToken))
    //            throw new ArgumentException("Invalid token. Token cannot be null or empty.");

    //        await _localStorageService.SetItemAsync(RefreshTokenKey, refreshToken);
    //    }

    //    public async Task<string> RefreshAccessTokenAsync(string refreshToken)
    //    {
    //        // Implement the actual token refresh logic here, calling the identity provider's token refresh endpoint
    //        // Assuming that new tokens are returned and then stored
    //        var newAccessToken = "new_access_token"; // Placeholder for new access token
    //        await StoreAccessTokenAsync(newAccessToken);
    //        return newAccessToken;
    //    }

    //    public async Task ClearTokenAsync()
    //    {
    //        await _localStorageService.RemoveItemAsync(AccessTokenKey);
    //        await _localStorageService.RemoveItemAsync(RefreshTokenKey);
    //        _appState.AccessToken = null;
    //    }

    //    public bool NeedsRefresh(string refreshToken)
    //    {
    //        // Implement logic to determine if the refresh token needs refreshing, possibly based on expiry
    //        return true; // Placeholder implementation
    //    }

    //    private async void RefreshTokenIfNeeded()
    //    {
    //        var refreshToken = await GetRefreshTokenAsync();
    //        if (refreshToken != null && NeedsRefresh(refreshToken))
    //        {
    //            await RefreshAccessTokenAsync(refreshToken);
    //            //await _authenticationStateProvider.RefreshToken();
    //            //_authenticationStateProvider.NotifyAuthenticationStateChange(GetAuthenticationStateAsync());
    //        }
    //    }

    //    //private Task<AuthenticationState> GetAuthenticationStateAsync()
    //    //{
    //    //    return _authenticationStateProvider.GetAuthenticationStateAsync();
    //    //}
    //}
    //public interface ITokenService
    //{
    //    Task<string> GetAccessTokenAsync();
    //    Task<string> GetRefreshTokenAsync();
    //    Task<string> RefreshAccessTokenAsync(string refreshToken);
    //    bool NeedsRefresh(string refreshToken);
    //}

    //public class TokenManager
    //{
    //    private readonly IAccessTokenProvider _tokenProvider;
    //    private AccessToken _cachedToken;

    //    public TokenManager(IAccessTokenProvider tokenProvider)
    //    {
    //        _tokenProvider = tokenProvider;
    //    }

    //    public async Task<string> GetAccessTokenAsync()
    //    {
    //        if (_cachedToken != null && !_cachedToken.Expires.InThePast())
    //        {
    //            return _cachedToken.Value;
    //        }

    //        var tokenResult = await _tokenProvider.RequestAccessToken();
    //        if (tokenResult.TryGetToken(out var token))
    //        {
    //            _cachedToken = token;
    //            return token.Value;
    //        }
    //        else
    //        {
    //            throw new InvalidOperationException("Unable to obtain access token. Please check your authentication settings.");
    //        }
    //    }
    //    //public async Task<string> TryAcquireTokenSilently()
    //    //{
    //    //    var accounts = await _msalClient.GetAccountsAsync();
    //    //    try
    //    //    {

    //    //        var result = await _msalClient.AcquireTokenSilent(_scopes, accounts.FirstOrDefault())
    //    //                                      .ExecuteAsync();
    //    //    }
    //    //    catch (MsalUiRequiredException ex)
    //    //    {
    //    //        if (ex.Claims != null)
    //    //        {
    //    //            // Handle conditional access claims challenge
    //    //            await _msalClient.AcquireTokenInteractive(_scopes)
    //    //                             .WithClaims(ex.Claims)
    //    //                             .ExecuteAsync();
    //    //        }
    //    //    }
    //    //    return result.AccessToken;
    //    //}
    //    //public void ConfigureTokenCache(ITokenCache tokenCache)
    //    //{
    //    //    tokenCache.SetBeforeAccess(BeforeAccessNotification);
    //    //    tokenCache.SetAfterAccess(AfterAccessNotification);
    //    //}
    //    //private void BeforeAccessNotification(TokenCacheNotificationArgs args)
    //    //{
    //    //    // Load the cache data from a secure location
    //    //    args.TokenCache.DeserializeMsalV3(secureStorage.Load());
    //    //}

    //    //private void AfterAccessNotification(TokenCacheNotificationArgs args)
    //    //{
    //    //    // Persist changes to the cache data
    //    //    if (args.HasStateChanged)
    //    //    {
    //    //        secureStorage.Save(args.TokenCache.SerializeMsalV3());
    //    //    }
    //    //}
    //    //public async Task<IEnumerable<IAccount>> GetAccountsAsync()
    //    //{
    //    //    return await _msalClient.GetAccountsAsync();
    //    //}

    //    //public async Task RemoveAccountAsync(IAccount account)
    //    //{
    //    //    await _msalClient.RemoveAsync(account);
    //    //}
    //    //public void ConfigureHttpClient()
    //    //{
    //    //    _msalClient.ClientApplicationBase.AppConfig.HttpClientFactory = new CustomHttpClientFactory();
    //    //}
    //    //public async Task<string> AcquireTokenByIntegratedWindowsAuth(string username)
    //    //{
    //    //    var result = await _msalClient.AcquireTokenByIntegratedWindowsAuth(_scopes)
    //    //                                  .WithUsername(username)
    //    //                                  .ExecuteAsync();
    //    //    return result.AccessToken;
    //    //}
    //    //public void EnableBroker()
    //    //{
    //    //    _msalClient.WithBroker(true);
    //    //}
    //    //public async Task<string> AcquireTokenForPolicy(string policy)
    //    //{
    //    //    var authority = $"https://{yourTenant}.b2clogin.com/{yourTenant}.onmicrosoft.com/{policy}";
    //    //    var pca = PublicClientApplicationBuilder.Create(clientId)
    //    //                                            .WithB2CAuthority(authority)
    //    //                                            .Build();
    //    //    var result = await pca.AcquireTokenInteractive(_scopes).ExecuteAsync();
    //    //    return result.AccessToken;
    //    //}
    //    //public void AddEventHandlers()
    //    //{
    //    //    _msalClient.ClientApplicationBase.AddAcquireTokenStartedNotification(context =>
    //    //    {
    //    //        Log.Information("Token acquisition started");
    //    //    });

    //    //    _msalClient.ClientApplicationBase.AddAcquireTokenCompletedNotification(context =>
    //    //    {
    //            //Log.Information("Token acquisition completed");
    //    //    });
    //    //}
    //}
