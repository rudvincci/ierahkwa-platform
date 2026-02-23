using System.Security.Claims;
using Mamey.Identity.AspNetCore.Providers;
using Mamey.Types;
using Microsoft.AspNetCore.Http;

namespace Mamey.Identity.AspNetCore.Providers;

public sealed class HttpContextTenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _http;
    private readonly TenantId _fallbackTenant;
    private readonly UserId   _fallbackUser;

    public HttpContextTenantProvider(
        IHttpContextAccessor http,
        TenantId? fallbackTenant = null,
        UserId?   fallbackUser   = null)
    {
        _http           = http ?? throw new ArgumentNullException(nameof(http));
        _fallbackTenant = fallbackTenant ?? new TenantId(Guid.Empty);
        _fallbackUser   = fallbackUser   ?? UserId.Empty;
    }

    public TenantId CurrentTenantId => TryGetTenantId(out var tid) ? tid : _fallbackTenant;
    public UserId   CurrentUserId   => TryGetUserId(out var uid) ? uid : _fallbackUser;

    public bool TryGetTenantId(out TenantId tenantId)
    {
        tenantId = Guid.Empty;
        var ctx = _http.HttpContext;
        if (ctx == null) return false;

        // 1) Items
        if (ctx.Items.TryGetValue("TenantId", out var obj) && TryGuid(obj, out var g))
        { tenantId = new TenantId(g); return true; }

        // 2) Route
        if (ctx.Request.RouteValues.TryGetValue("tenantId", out var rv) && TryGuid(rv, out g))
        { tenantId = new TenantId(g); return true; }

        // 3) Header
        if (ctx.Request.Headers.TryGetValue("X-Tenant-Id", out var hs) && Guid.TryParse(hs, out g))
        { tenantId = new TenantId(g); return true; }

        // 4) Claim
        var claim = ctx.User?.FindFirst("TenantId");
        if (claim != null && Guid.TryParse(claim.Value, out g))
        { tenantId = new TenantId(g); return true; }

        return false;
    }

    public bool TryGetUserId(out UserId userId)
    {
        userId = UserId.Empty;
        var ctx = _http.HttpContext;
        if (ctx == null) return false;

        if (ctx.Items.TryGetValue("UserId", out var obj) && TryGuid(obj, out var g))
        { userId = new UserId(g); return true; }

        if (ctx.Request.Headers.TryGetValue("X-User-Id", out var hs) && Guid.TryParse(hs, out g))
        { userId = new UserId(g); return true; }

        var claim = ctx.User?.FindFirst(ClaimTypes.NameIdentifier);
        if (claim != null && Guid.TryParse(claim.Value, out g))
        { userId = new UserId(g); return true; }

        return false;
    }

    private static bool TryGuid(object? raw, out Guid g)
    {
        switch (raw)
        {
            case Guid guid: g = guid; return true;
            case string s when Guid.TryParse(s, out var parsed): g = parsed; return true;
            default: g = default; return false;
        }
    }
}

