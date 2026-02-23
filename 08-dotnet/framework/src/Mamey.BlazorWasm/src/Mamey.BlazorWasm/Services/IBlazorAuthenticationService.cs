namespace Mamey.BlazorWasm;

//public class AuthenticationService
//{
//    private readonly IAccessTokenProvider _accessTokenProvider;
//    private IPublicClientApplication _msalClient;
//    private string[] _scopes;

//    public AuthenticationService(IAccessTokenProvider accessTokenProvider)
//    {
//        _accessTokenProvider = accessTokenProvider;
//    }

//    public async Task<string> GetAccessTokenAsync()
//    {
//        var result = await _accessTokenProvider.RequestAccessToken();

//        if (result.TryGetToken(out var token))
//        {
//            return token.Value;
//        }
//        else
//        {
//            throw new InvalidOperationException("Unable to obtain access token.");
//        }
//    }
//    public async Task<string> GetAccessTokenAsync()
//    {
//        try
//        {
//            var accounts = await _msalClient.GetAccountsAsync();
//            var result = await _msalClient.AcquireTokenSilent(_scopes, accounts.FirstOrDefault())
//                                          .ExecuteAsync();

//            return result.AccessToken;
//        }
//        catch (MsalUiRequiredException)
//        {
//            // This exception is thrown when an interactive sign-in is required.
//            // Handle accordingly for your application scenario, possibly initiating an interactive sign-in.
//            throw;
//        }
//    }
//}

