using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using Mamey.Portal.Shared.Tenancy;
using Mamey.Portal.Web.Auth;

namespace Mamey.Portal.Web.Tenancy;

public sealed class ClaimsTenantContext : ITenantContext
{
    private static readonly System.Text.RegularExpressions.Regex InvalidTenantChars =
        new(@"[^a-z0-9\-]+", System.Text.RegularExpressions.RegexOptions.Compiled | System.Text.RegularExpressions.RegexOptions.IgnoreCase);

    private readonly IHttpContextAccessor _http;
    private readonly AuthenticationStateProvider? _auth;
    private readonly PortalAuthOptions _options;

    public ClaimsTenantContext(IHttpContextAccessor http, AuthenticationStateProvider auth, IOptions<PortalAuthOptions> options)
    {
        _http = http;
        _auth = auth;
        _options = options.Value;
    }

    private System.Security.Claims.ClaimsPrincipal Principal
    {
        get
        {
            // In normal HTTP request pipeline (endpoints/middleware), HttpContext is always available.
            var httpUser = _http.HttpContext?.User;
            if (httpUser is not null)
            {
                return httpUser;
            }

            // In Blazor Server circuits, HttpContext can be null. Fall back to AuthenticationStateProvider,
            // but DO NOT call it eagerly (it throws if used outside a Razor component DI scope).
            try
            {
                return _auth?.GetAuthenticationStateAsync().GetAwaiter().GetResult().User
                       ?? new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity());
            }
            catch
            {
                return new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity());
            }
        }
    }

    public string TenantId
    {
        get
        {
            if (_http.HttpContext?.Items.TryGetValue("ResolvedTenantId", out var mapped) == true
                && mapped is string mappedTenant
                && !string.IsNullOrWhiteSpace(mappedTenant))
            {
                return NormalizeTenantId(mappedTenant);
            }

            var claimType = string.IsNullOrWhiteSpace(_options.TenantClaimType) ? "tenant" : _options.TenantClaimType;
            var tenant = Principal.FindFirst(claimType)?.Value;
            tenant = NormalizeTenantId(tenant);
            return string.IsNullOrWhiteSpace(tenant) ? "default" : tenant;
        }
    }

    public static string NormalizeTenantId(string? tenantId)
    {
        tenantId = (tenantId ?? string.Empty).Trim().ToLowerInvariant();
        tenantId = tenantId.Replace(' ', '-');
        tenantId = InvalidTenantChars.Replace(tenantId, "");
        tenantId = tenantId.Trim('-');
        return tenantId.Length > 128 ? tenantId[..128] : tenantId;
    }
}


