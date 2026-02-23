using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Services;

/// <summary>
/// Implementation of IContext that gets user info from HttpContext.
/// </summary>
internal sealed class Context : IContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public Context(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public string UserId => User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? User?.FindFirst("sub")?.Value
        ?? "system";

    public string? DisplayName => User?.FindFirst("preferred_username")?.Value
        ?? User?.FindFirst(ClaimTypes.Name)?.Value
        ?? User?.Identity?.Name;

    public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value
        ?? User?.FindFirst("email")?.Value;

    public Guid? TenantId
    {
        get
        {
            // First, try to get from X-TENANT-ID header (set by UI services)
            var tenantHeader = GetHeader("X-TENANT-ID");
            if (!string.IsNullOrWhiteSpace(tenantHeader) && Guid.TryParse(tenantHeader, out var tenantIdFromHeader))
            {
                return tenantIdFromHeader;
            }

            // Fall back to "tenant" claim from user identity
            var tenantClaim = User?.FindFirst("tenant")?.Value;
            if (!string.IsNullOrWhiteSpace(tenantClaim) && Guid.TryParse(tenantClaim, out var tenantIdFromClaim))
            {
                return tenantIdFromClaim;
            }

            return null;
        }
    }

    public string? IpAddress
    {
        get
        {
            var forwarded = GetHeader("X-Forwarded-For");
            if (!string.IsNullOrWhiteSpace(forwarded))
            {
                var first = forwarded.Split(',').FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(first))
                {
                    return first.Trim();
                }
            }

            var realIp = GetHeader("X-Real-IP");
            if (!string.IsNullOrWhiteSpace(realIp))
            {
                return realIp.Trim();
            }

            return _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        }
    }

    public string? UserAgent => _httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString();

    public string? BaseUrl
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return null;

            var request = httpContext.Request;
            var scheme = request.Scheme;
            var host = request.Host.ToUriComponent();
            var pathBase = request.PathBase.ToUriComponent();

            return $"{scheme}://{host}{pathBase}";
        }
    }

    public string? DeviceType => GetHeader("X-Device-Type");
    public string? DeviceId => GetHeader("X-Device-Id");
    public string? MacAddress => GetHeader("X-Mac-Address");
    public string? Platform => GetHeader("X-Platform") ?? GetHeader("Sec-CH-UA-Platform");
    public string? Browser => GetHeader("X-Browser") ?? GetHeader("Sec-CH-UA");
    public string? OsVersion => GetHeader("X-OS-Version");
    public string? AppVersion => GetHeader("X-App-Version");
    public string? ScreenResolution => GetHeader("X-Screen-Resolution");
    public string? Language => GetHeader("Accept-Language");
    public string? Timezone => GetHeader("X-Timezone");
    public string? Referrer => GetHeader("Referer");
    public string? NetworkType => GetHeader("X-Network-Type");
    public double? Latitude => ParseDoubleHeader("X-Client-Latitude");
    public double? Longitude => ParseDoubleHeader("X-Client-Longitude");

    private string? GetHeader(string name)
        => _httpContextAccessor.HttpContext?.Request.Headers[name].ToString();

    private double? ParseDoubleHeader(string name)
    {
        var raw = GetHeader(name);
        return double.TryParse(raw, out var value) ? value : null;
    }
}
