using Microsoft.AspNetCore.Http;

namespace Mamey.Portal.Web.Auth;

public sealed class MockAuthSession
{
    public const string CookieName = "mamey.mock.sid";

    private readonly IHttpContextAccessor _http;
    private readonly MockSessionStore _store;

    public MockAuthSession(IHttpContextAccessor http, MockSessionStore store)
    {
        _http = http;
        _store = store;
    }

    private string SessionId
    {
        get
        {
            var ctx = _http.HttpContext;
            var sid = ctx?.Request.Cookies[CookieName];
            return string.IsNullOrWhiteSpace(sid) ? "anon" : sid;
        }
    }

    private MockSessionData Data => _store.GetOrCreate(SessionId);

    public string Tenant => Data.Tenant;
    public string UserName => Data.UserName;
    public string Role => Data.Role;

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(UserName) && !string.IsNullOrWhiteSpace(Role);

    public void SignIn(string tenant, string userName, string role)
    {
        Data.Tenant = string.IsNullOrWhiteSpace(tenant) ? "default" : tenant.Trim();
        Data.UserName = userName.Trim();
        Data.Role = role.Trim();
    }

    public void SignOut()
    {
        Data.UserName = string.Empty;
        Data.Role = string.Empty;
    }
}


