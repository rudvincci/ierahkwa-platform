using Microsoft.AspNetCore.Http;
using SpikeOffice.Infrastructure.Services;

namespace SpikeOffice.Infrastructure.Middleware;

/// <summary>
/// Resolves tenant from path: /t/{urlPrefix}/... or from host/header.
/// IERAHKWA: /t/ierahkwa-pm/..., /t/ierahkwa-mft/...
/// </summary>
public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext ctx, TenantContext tenantCtx, Core.Interfaces.ITenantService tenantSvc)
    {
        // 1) Path: /t/{urlPrefix}/...
        var path = ctx.Request.Path.Value ?? "";
        var segments = path.TrimStart('/').Split('/');
        if (segments.Length >= 2 && segments[0].Equals("t", StringComparison.OrdinalIgnoreCase))
        {
            var urlPrefix = segments[1];
            var tenant = await tenantSvc.GetByUrlPrefixAsync(urlPrefix, ctx.RequestAborted);
            if (tenant != null)
            {
                tenantCtx.TenantId = tenant.Id;
                tenantCtx.TenantUrlPrefix = tenant.UrlPrefix;
                // Rewrite path to remove /t/{prefix} for routing
                var rest = string.Join("/", segments.Skip(2));
                ctx.Request.Path = new PathString(string.IsNullOrEmpty(rest) ? "/" : "/" + rest);
            }
        }
        // 2) Header: X-Tenant-Prefix (for SPA / API)
        else if (ctx.Request.Headers.TryGetValue("X-Tenant-Prefix", out var prefix) && !string.IsNullOrEmpty(prefix))
        {
            var tenant = await tenantSvc.GetByUrlPrefixAsync(prefix!, ctx.RequestAborted);
            if (tenant != null)
            {
                tenantCtx.TenantId = tenant.Id;
                tenantCtx.TenantUrlPrefix = tenant.UrlPrefix;
            }
        }
            // SECURITY: Query parameter tenant resolution disabled (OWASP A01 - Broken Access Control)
            // Tenant must be resolved from subdomain, path, or authenticated user context only

        await _next(ctx);
    }
}
