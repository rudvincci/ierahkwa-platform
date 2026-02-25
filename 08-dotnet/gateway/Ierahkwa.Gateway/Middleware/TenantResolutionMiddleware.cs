namespace Ierahkwa.Gateway.Middleware;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Resolve tenant from: 1) Header, 2) JWT claim, 3) Subdomain
        var tenantId = ResolveTenantId(context);

        if (!string.IsNullOrEmpty(tenantId))
        {
            context.Items["TenantId"] = tenantId;
            context.Request.Headers["X-Tenant-Id"] = tenantId;
        }

        await _next(context);
    }

    private static string? ResolveTenantId(HttpContext context)
    {
        // 1. Explicit header
        if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var headerTenant))
            return headerTenant.ToString();

        // 2. JWT claim
        var tenantClaim = context.User?.FindFirst("tenant")?.Value;
        if (!string.IsNullOrEmpty(tenantClaim))
            return tenantClaim;

        // 3. Subdomain (e.g., navajo.ierahkwa.org → navajo)
        var host = context.Request.Host.Host;
        var parts = host.Split('.');
        if (parts.Length >= 3 && parts[0] != "www" && parts[0] != "api")
            return parts[0];

        return null;
    }
}
