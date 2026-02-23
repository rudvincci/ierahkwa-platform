using Microsoft.Extensions.Options;

namespace MameyNode.Portals.Web.Infrastructure;

public class PortalAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly Dictionary<string, PortalSettings> _portalSettings;

    public PortalAuthMiddleware(RequestDelegate next, IOptions<Dictionary<string, PortalSettings>> portalSettings)
    {
        _next = next;
        _portalSettings = portalSettings.Value ?? new Dictionary<string, PortalSettings>();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path;
        
        // Identify which portal is being accessed
        if (_portalSettings != null)
        {
            foreach (var portal in _portalSettings)
            {
                var settings = portal.Value;
                if (settings?.Routes != null && settings.Routes.Length > 0)
                {
                    // Simple prefix match
                    if (settings.Routes.Any(r => path.StartsWithSegments(r)))
                    {
                        context.Items["CurrentPortal"] = portal.Key;
                        
                        // In a real implementation, we would check context.User.Identities
                        // or authentication schemes to ensure compliance with settings.AuthMethods
                        
                        break;
                    }
                }
            }
        }

        await _next(context);
    }
}



