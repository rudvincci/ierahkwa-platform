using Microsoft.AspNetCore.Builder;

namespace Mamey.ApplicationName.Modules.Identity.Api.Routes;

/// <summary>
/// Extension methods for minimal‑API permission conventions.
/// </summary>
public static class EndpointConventions
{
    /// <summary>
    /// Applies a "Permission:{permission}" policy to this endpoint.
    /// </summary>
    public static RouteHandlerBuilder RequirePermission(
        this RouteHandlerBuilder builder,
        string permission)
    {
        return builder.RequireAuthorization(
            policy => policy.RequireClaim("permission", permission));
    }
}