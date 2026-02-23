using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Mamey.Government.Modules.Identity.Blazor.Services;

/// <summary>
/// Implementation of authentication service for Authentik OIDC.
/// </summary>
public class IdentityAuthService : IIdentityAuthService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<IdentityAuthService> _logger;

    public IdentityAuthService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<IdentityAuthService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public Task<ClaimsPrincipal?> GetCurrentUserAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User != null && httpContext.User.Identity?.IsAuthenticated == true)
        {
            return Task.FromResult<ClaimsPrincipal?>(httpContext.User);
        }

        return Task.FromResult<ClaimsPrincipal?>(null);
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var user = await GetCurrentUserAsync();
        return user?.Identity?.IsAuthenticated ?? false;
    }

    public async Task<string?> GetDisplayNameAsync()
    {
        var user = await GetCurrentUserAsync();
        return user?.FindFirst("preferred_username")?.Value 
            ?? user?.FindFirst(ClaimTypes.Name)?.Value
            ?? user?.Identity?.Name;
    }

    public async Task<string?> GetEmailAsync()
    {
        var user = await GetCurrentUserAsync();
        return user?.FindFirst(ClaimTypes.Email)?.Value
            ?? user?.FindFirst("email")?.Value;
    }

    public async Task<Guid?> GetTenantIdAsync()
    {
        var user = await GetCurrentUserAsync();
        var tenantClaim = user?.FindFirst("tenant")?.Value;
        
        if (Guid.TryParse(tenantClaim, out var tenantId))
        {
            return tenantId;
        }

        return null;
    }

    public async Task SignOutAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            // Sign out from Authentik OIDC
            await httpContext.SignOutAsync("Cookies");
            await httpContext.SignOutAsync("oidc");
        }
    }
}
