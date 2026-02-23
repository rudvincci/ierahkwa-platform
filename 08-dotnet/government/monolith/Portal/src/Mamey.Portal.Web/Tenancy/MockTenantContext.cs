using Mamey.Portal.Shared.Tenancy;
using Mamey.Portal.Web.Auth;

namespace Mamey.Portal.Web.Tenancy;

public sealed class MockTenantContext : ITenantContext
{
    private readonly MockAuthSession _session;

    public MockTenantContext(MockAuthSession session)
    {
        _session = session;
    }

    public string TenantId => string.IsNullOrWhiteSpace(_session.Tenant) ? "default" : _session.Tenant;
}




