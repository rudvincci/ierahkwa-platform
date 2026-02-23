using Mamey.Portal.Shared.Auth;

namespace Mamey.Portal.Web.Auth;

public sealed class MockCurrentUserContext : ICurrentUserContext
{
    private readonly MockAuthSession _session;

    public MockCurrentUserContext(MockAuthSession session)
    {
        _session = session;
    }

    public string UserName => _session.UserName;
    public string Role => _session.Role;
    public bool IsAuthenticated => _session.IsAuthenticated;
}




