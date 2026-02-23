using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mamey.Government.BlazorServer.Auth;

/// <summary>
/// Development-only authentication endpoints for mock login.
/// These endpoints are disabled in production.
/// </summary>
[ApiController]
[Route("dev")]
public class DevAuthController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<DevAuthController> _logger;

    public DevAuthController(IWebHostEnvironment env, ILogger<DevAuthController> logger)
    {
        _env = env;
        _logger = logger;
    }

    /// <summary>
    /// Mock login endpoint for development.
    /// Usage: /dev/mock-login?tenant=default&amp;user=admin@example.com&amp;role=Admin
    /// </summary>
    [HttpGet("mock-login")]
    public async Task<IActionResult> MockLogin(
        [FromQuery] string tenant = "default",
        [FromQuery] string user = "dev@example.com",
        [FromQuery] string role = "Citizen",
        [FromQuery] string? returnUrl = "/")
    {
        if (!_env.IsDevelopment())
        {
            return NotFound("This endpoint is only available in development mode.");
        }

        _logger.LogInformation("Mock login: user={User}, role={Role}, tenant={Tenant}", user, role, tenant);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, user),
            new(ClaimTypes.Email, user),
            new(ClaimTypes.Role, role),
            new("preferred_username", user),
            new("tenant", tenant),
            new("sub", Guid.NewGuid().ToString()),
        };

        // Add multiple roles if comma-separated
        if (role.Contains(','))
        {
            foreach (var r in role.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                if (!claims.Any(c => c.Type == ClaimTypes.Role && c.Value == r))
                {
                    claims.Add(new Claim(ClaimTypes.Role, r));
                }
            }
        }

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            });

        _logger.LogInformation("Mock login successful for {User}", user);

        // SEC-05: Validate returnUrl to prevent open redirect
        var safeReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : "/";
        return Redirect(safeReturnUrl);
    }

    /// <summary>
    /// Returns current user claims for debugging.
    /// </summary>
    [HttpGet("whoami")]
    public IActionResult WhoAmI()
    {
        if (!_env.IsDevelopment())
        {
            return NotFound("This endpoint is only available in development mode.");
        }

        if (User.Identity?.IsAuthenticated != true)
        {
            return Ok(new
            {
                authenticated = false,
                message = "Not logged in"
            });
        }

        var claims = User.Claims.Select(c => new { type = c.Type, value = c.Value }).ToList();
        var roles = User.Claims
            .Where(c => c.Type == ClaimTypes.Role || c.Type == "roles" || c.Type == "role")
            .Select(c => c.Value)
            .ToList();

        return Ok(new
        {
            authenticated = true,
            name = User.Identity.Name,
            nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            email = User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value,
            tenant = User.FindFirst("tenant")?.Value,
            roles,
            allClaims = claims
        });
    }

    /// <summary>
    /// Mock logout endpoint for development.
    /// </summary>
    [HttpGet("mock-logout")]
    public async Task<IActionResult> MockLogout([FromQuery] string? returnUrl = "/")
    {
        if (!_env.IsDevelopment())
        {
            return NotFound("This endpoint is only available in development mode.");
        }

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        _logger.LogInformation("Mock logout successful");

        // SEC-05: Validate returnUrl to prevent open redirect
        var safeReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : "/";
        return Redirect(safeReturnUrl);
    }
}
