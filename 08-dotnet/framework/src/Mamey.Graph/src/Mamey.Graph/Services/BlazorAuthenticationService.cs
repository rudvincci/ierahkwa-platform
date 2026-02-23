using Microsoft.JSInterop;
namespace Mamey.Graph;

public class BlazorAuthenticationService : IBlazorAuthenticationService
{
    private readonly IJSRuntime _jsRuntime;

    public BlazorAuthenticationService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task SaveAccessToken(string accessToken)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "accessToken", accessToken);
    }

    public async Task<string> GetAccessToken()
    {
        return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "accessToken");
    }

}









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
//        // _authenticationStateProvider = (TokenAuthenticationStateProvider)authenticationStateProvider;
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
