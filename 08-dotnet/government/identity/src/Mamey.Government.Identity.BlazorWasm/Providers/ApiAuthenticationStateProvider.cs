using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Mamey.Auth;
using Mamey.Government.Identity.BlazorWasm.Storage;
using Microsoft.AspNetCore.Components.Authorization;

namespace Mamey.Government.Identity.BlazorWasm.Providers;

public class ApiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ITokenStore _store;
    private string? _token;
    private ClaimsPrincipal _principal = new(new ClaimsIdentity());
    private readonly TaskCompletionSource _readyTcs = new();

    public ApiAuthenticationStateProvider(ITokenStore store) => _store = store;

    public string? CurrentToken => _token;
    public ClaimsPrincipal CurrentUser => _principal;
    public AuthenticatedUser? AuthenticatedUser { get; private set; }
    public Task WhenReady => _readyTcs.Task;

    public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
        Task.FromResult(new AuthenticationState(_principal));

    // Call this from App.razor OnAfterRenderAsync(firstRender) only
    public async Task InitializeAsync(bool persist = true)
    {
        var token = await _store.GetAsync();             // safe after first render
        await SetTokenAsync(token, persist: persist);       // don't re-persist on init
        _readyTcs.TrySetResult();
    }

    public async Task SetTokenAsync(string? token, bool persist = true)
    {
        _token = token;

        if (string.IsNullOrWhiteSpace(token))
        {
            _principal = new ClaimsPrincipal(new ClaimsIdentity());
            AuthenticatedUser = null;
            if (persist) await _store.ClearAsync();
        }
        else
        {
            var raw = Mamey.Auth.Identity.Extensions.ParseClaimsFromJwt(token) ?? Enumerable.Empty<Claim>();

            // Normalize roles → ClaimTypes.Role
            var normalized = new List<Claim>();
            foreach (var c in raw)
            {
                if (c.Type.Equals("roles", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var v in c.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                        normalized.Add(new Claim(ClaimTypes.Role, v));
                }
                else if (c.Type.Equals("role", StringComparison.OrdinalIgnoreCase) ||
                         c.Type == ClaimTypes.Role)
                {
                    normalized.Add(new Claim(ClaimTypes.Role, c.Value));
                }
                else
                {
                    normalized.Add(c);
                }
            }

            var identity = new ClaimsIdentity(
                raw ?? Enumerable.Empty<Claim>(),
                authenticationType: "jwt",
                nameType: ClaimTypes.Name,          // map below if missing
                roleType: ClaimTypes.Role);

            // If Name claim missing, try common fallbacks
            if (!identity.HasClaim(c => c.Type == ClaimTypes.Name))
            {
                var name = normalized.FirstOrDefault(c =>
                               c.Type == "name" ||
                               c.Type == JwtRegisteredClaimNames.UniqueName ||
                               c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")
                           ?.Value;
                if (!string.IsNullOrWhiteSpace(name))
                    identity.AddClaim(new Claim(ClaimTypes.Name, name));
            }

            _principal = new ClaimsPrincipal(identity);

            // Build optional AuthenticatedUser safely
            var sub = _principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                      ?? _principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid.TryParse(sub, out var userId);

            var nameFull = _principal.FindFirst("fullName")?.Value
                           ?? _principal.Identity?.Name ?? "User";
            var role = _principal.FindFirst(ClaimTypes.Role)?.Value ?? "";
            var email = _principal.FindFirst(ClaimTypes.Email)?.Value ?? "";
            var expStr = _principal.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;
            long.TryParse(expStr, out var expires);

            AuthenticatedUser = new AuthenticatedUser
            {
                UserId = userId,
                Claims = _principal.Claims.GroupBy(c => c.Type)
                    .ToDictionary(g => g.Key, g => string.Join(",", g.Select(c => c.Value))),
                TenantId = null,
                Name = nameFull,
                Role = role,
                Expires = expires,
                Email = email,
                AccessToken = token,
            };

            if (persist) await _store.SaveAsync(token);
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_principal)));
        await Task.CompletedTask;
    }

    public async Task LogoutAsync()
    {
        _principal = new ClaimsPrincipal(new ClaimsIdentity());
        _token = null;
        AuthenticatedUser = null;
        await _store.ClearAsync();
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_principal)));
    }
}