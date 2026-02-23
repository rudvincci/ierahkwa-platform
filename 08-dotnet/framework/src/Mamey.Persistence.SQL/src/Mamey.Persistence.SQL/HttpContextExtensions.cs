using Microsoft.AspNetCore.Http;

namespace Mamey.Persistence.SQL;

public static class HttpContextExtensions
{
    private const string TenantKey = "TenantId";

    public static void SetTenantId(this HttpContext ctx, Guid tenantId)
        => ctx.Items[TenantKey] = tenantId;

    public static Guid GetTenantId(this HttpContext ctx)
    {
        if (ctx.Items.TryGetValue(TenantKey, out var val)
            && val is Guid gid)
            return gid;
        throw new InvalidOperationException("TenantId not set in context.");
    }
}