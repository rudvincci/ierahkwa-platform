namespace Mamey.Graph.Msal;
public class MsalAuthenticationResult
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public string IdToken { get; set; }
    public DateTimeOffset ExpiresOn { get; set; }
    public string[] Scopes { get; set; }
    public IAccount Account { get; set; }

    public MsalAuthenticationResult(string accessToken, string refreshToken, string idToken, DateTimeOffset expiresOn, string[] scopes, IAccount account)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        IdToken = idToken;
        ExpiresOn = expiresOn;
        Scopes = scopes;
        Account = account;
    }
}
