using Microsoft.AspNetCore.Components.Authorization;
using Mamey.Portal.Shared.Auth;

namespace Mamey.Portal.Web.Auth;

public sealed class ClaimsCurrentUserContext : ICurrentUserContext
{
    private readonly IHttpContextAccessor _http;
    private readonly AuthenticationStateProvider? _auth;

    public ClaimsCurrentUserContext(IHttpContextAccessor http, AuthenticationStateProvider auth)
    {
        _http = http;
        _auth = auth;
    }

    private System.Security.Claims.ClaimsPrincipal Principal
    {
        get
        {
            var httpUser = _http.HttpContext?.User;
            if (httpUser is not null)
            {
                return httpUser;
            }

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

    public string UserName
        => Principal.Identity?.Name
           ?? Principal.FindFirst("preferred_username")?.Value
           ?? Principal.FindFirst("email")?.Value
           ?? string.Empty;

    public string Role
        => Roles.FirstOrDefault() ?? string.Empty;

    public IReadOnlyCollection<string> Roles
        => ExtractRoles(Principal);

    public bool IsAuthenticated => Principal.Identity?.IsAuthenticated == true;

    private static IReadOnlyCollection<string> ExtractRoles(System.Security.Claims.ClaimsPrincipal principal)
    {
        var values = new List<string>();

        foreach (var c in principal.Claims)
        {
            if (c.Type is not ("roles" or "role" or System.Security.Claims.ClaimTypes.Role))
            {
                continue;
            }

            AddRoleValues(values, c.Value);
        }

        return values
            .Select(x => (x ?? string.Empty).Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static void AddRoleValues(List<string> target, string value)
    {
        value = (value ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(value)) return;

        // Authentik may emit arrays as JSON (e.g. ["Admin","ContentEditor"]) or simple CSV.
        if (value.StartsWith("[", StringComparison.Ordinal) && value.EndsWith("]", StringComparison.Ordinal))
        {
            try
            {
                var arr = System.Text.Json.JsonSerializer.Deserialize<string[]>(value);
                if (arr is not null)
                {
                    target.AddRange(arr);
                    return;
                }
            }
            catch
            {
                // fall through
            }
        }

        if (value.Contains(',', StringComparison.Ordinal))
        {
            foreach (var part in value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                target.Add(part);
            }

            return;
        }

        target.Add(value);
    }
}


