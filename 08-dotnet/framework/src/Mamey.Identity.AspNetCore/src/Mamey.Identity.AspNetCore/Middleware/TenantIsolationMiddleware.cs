using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Mamey.Types;

namespace Mamey.Identity.AspNetCore.Middleware;

/// <summary>
/// Resolves TenantId from multiple sources (Items, header, route, claim, query) and stores it in HttpContext.Items["TenantId"].
/// Use with ITenantProvider that reads from Items.
/// </summary>
public sealed class TenantIsolationMiddleware : IMiddleware
{
    public const string ItemKey   = "TenantId";
    public const string HeaderKey = "X-Tenant-Id";
    public const string RouteKey  = "tenantId";
    public const string QueryKey  = "tenantId";
    public const string ClaimKey  = "TenantId";

    private readonly ILogger<TenantIsolationMiddleware> _log;

    public TenantIsolationMiddleware(ILogger<TenantIsolationMiddleware> log) => _log = log;

    public async Task InvokeAsync(HttpContext ctx, RequestDelegate next)
    {
        // Already set by an upstream component?
        if (ctx.Items.ContainsKey(ItemKey))
        {
            await next(ctx);
            return;
        }

        if (TryResolveTenant(ctx, out var tenantId))
        {
            ctx.Items[ItemKey] = tenantId;
            _log.LogDebug("Tenant resolved: {TenantId}", tenantId);
        }
        else
        {
            // Decide your policy: log & continue, or short‑circuit with 400/401.
            _log.LogWarning("Tenant not found for request {Path}", ctx.Request.Path);
            // Example: short circuit
            // ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
            // await ctx.Response.WriteAsync("Tenant header missing/invalid.");
            // return;
        }

        await next(ctx);
    }

    private static bool TryResolveTenant(HttpContext ctx, out TenantId tenantId)
    {
        tenantId = default;

        // 1) Items (upstream)
        if (ctx.Items.TryGetValue(ItemKey, out var v) && TryGuid(v, out var g))
        { tenantId = new TenantId(g); return true; }

        // 2) Header
        if (ctx.Request.Headers.TryGetValue(HeaderKey, out var h) && Guid.TryParse(h, out g))
        { tenantId = new TenantId(g); return true; }

        // 3) Route
        if (ctx.Request.RouteValues.TryGetValue(RouteKey, out var rv) && TryGuid(rv, out g))
        { tenantId = new TenantId(g); return true; }

        // 4) Query
        if (ctx.Request.Query.TryGetValue(QueryKey, out var q) && Guid.TryParse(q, out g))
        { tenantId = new TenantId(g); return true; }

        // 5) Claim
        var claim = ctx.User?.FindFirst(ClaimKey);
        if (claim != null && Guid.TryParse(claim.Value, out g))
        { tenantId = new TenantId(g); return true; }

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

