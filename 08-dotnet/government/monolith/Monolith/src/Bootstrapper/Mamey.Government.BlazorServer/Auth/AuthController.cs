using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Mamey.Government.BlazorServer.Auth;

/// <summary>
/// Authentication controller for login/logout endpoints.
/// Supports both Mock mode and OIDC (Authentik) mode.
/// </summary>
[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly GovernmentAuthOptions _authOptions;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IOptions<GovernmentAuthOptions> authOptions,
        ILogger<AuthController> logger)
    {
        _authOptions = authOptions.Value;
        _logger = logger;
    }

    private bool IsOidcMode => string.Equals(_authOptions.Mode, "Oidc", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Login page - redirects to OIDC or shows login options.
    /// </summary>
    [HttpGet("login")]
    public IActionResult Login([FromQuery] string? returnUrl = "/", [FromQuery] string? error = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return Redirect(returnUrl ?? "/");
        }

        if (IsOidcMode)
        {
            // In OIDC mode, redirect to OIDC login
            return RedirectToAction(nameof(OidcLogin), new { returnUrl });
        }

        // In Mock mode, show a simple login page or redirect to dev login helper
        return Content($@"
<!DOCTYPE html>
<html>
<head>
    <title>Login - Mamey Government Portal</title>
    <link href='https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap' rel='stylesheet'>
    <style>
        body {{ font-family: Roboto, sans-serif; background: #f5f5f5; display: flex; justify-content: center; align-items: center; height: 100vh; margin: 0; }}
        .card {{ background: white; padding: 40px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); max-width: 400px; width: 100%; }}
        h1 {{ color: #1565C0; margin-bottom: 20px; }}
        .btn {{ display: block; width: 100%; padding: 12px; margin: 10px 0; border: none; border-radius: 4px; cursor: pointer; font-size: 16px; text-decoration: none; text-align: center; }}
        .btn-primary {{ background: #1565C0; color: white; }}
        .btn-secondary {{ background: #00897B; color: white; }}
        .btn-outline {{ background: transparent; border: 2px solid #1565C0; color: #1565C0; }}
        .divider {{ margin: 20px 0; text-align: center; color: #999; }}
        .error {{ color: #d32f2f; margin-bottom: 15px; padding: 10px; background: #ffebee; border-radius: 4px; }}
    </style>
</head>
<body>
    <div class='card'>
        <h1>🏛️ Government Portal</h1>
        {(string.IsNullOrEmpty(error) ? "" : $"<div class='error'>Error: {error}</div>")}
        <p>Development Mode - Select a role to login:</p>
        <a class='btn btn-primary' href='/dev/mock-login?role=Admin&user=admin@example.com&tenant=default&returnUrl={Uri.EscapeDataString(returnUrl ?? "/")}'>Login as Admin</a>
        <a class='btn btn-secondary' href='/dev/mock-login?role=GovernmentAgent&user=agent@example.com&tenant=default&returnUrl={Uri.EscapeDataString(returnUrl ?? "/")}'>Login as Government Agent</a>
        <a class='btn btn-outline' href='/dev/mock-login?role=Citizen&user=citizen@example.com&tenant=default&returnUrl={Uri.EscapeDataString(returnUrl ?? "/")}'>Login as Citizen</a>
        <div class='divider'>— or —</div>
        <a class='btn btn-outline' href='/dev/mock-login?role=ContentEditor&user=editor@example.com&tenant=default&returnUrl={Uri.EscapeDataString(returnUrl ?? "/")}'>Login as Content Editor</a>
        <a class='btn btn-outline' href='/dev/mock-login?role=LibraryEditor&user=library@example.com&tenant=default&returnUrl={Uri.EscapeDataString(returnUrl ?? "/")}'>Login as Library Editor</a>
    </div>
</body>
</html>", "text/html");
    }

    /// <summary>
    /// OIDC login - initiates OpenID Connect authentication flow.
    /// </summary>
    [HttpGet("oidc/login")]
    public IActionResult OidcLogin([FromQuery] string? returnUrl = "/")
    {
        if (!IsOidcMode)
        {
            return RedirectToAction(nameof(Login), new { returnUrl });
        }

        _logger.LogInformation("Initiating OIDC login, returnUrl={ReturnUrl}", returnUrl);

        var properties = new AuthenticationProperties
        {
            RedirectUri = returnUrl ?? "/",
            IsPersistent = true
        };

        return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// Logout - signs out from cookie and optionally OIDC.
    /// </summary>
    [HttpGet("logout")]
    public async Task<IActionResult> Logout([FromQuery] string? returnUrl = "/")
    {
        _logger.LogInformation("Logging out user {User}", User.Identity?.Name);

        // Check if user has an OIDC session (id_token present means they logged in via OIDC)
        var idToken = await HttpContext.GetTokenAsync("id_token");
        var hasOidcSession = !string.IsNullOrEmpty(idToken);
        
        if (IsOidcMode && hasOidcSession)
        {
            _logger.LogInformation("User has OIDC session, performing OIDC logout");
            return RedirectToAction(nameof(OidcLogout), new { returnUrl });
        }

        // Cookie-only logout (mock login or no OIDC session)
        _logger.LogInformation("Performing cookie-only logout");
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect(returnUrl ?? "/");
    }

    /// <summary>
    /// OIDC logout - signs out from both cookie and OIDC provider.
    /// </summary>
    [HttpGet("oidc/logout")]
    public async Task<IActionResult> OidcLogout([FromQuery] string? returnUrl = "/")
    {
        // Check if user has an OIDC session
        var idToken = await HttpContext.GetTokenAsync("id_token");
        var hasOidcSession = !string.IsNullOrEmpty(idToken);
        
        if (!IsOidcMode || !hasOidcSession)
        {
            _logger.LogInformation("No OIDC session, redirecting to simple logout");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect(returnUrl ?? "/");
        }

        _logger.LogInformation("Initiating OIDC logout with id_token");

        var properties = new AuthenticationProperties
        {
            RedirectUri = returnUrl ?? "/"
        };

        // Sign out from both cookie and OIDC
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return SignOut(properties, OpenIdConnectDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// Returns authentication status.
    /// </summary>
    [HttpGet("status")]
    public IActionResult Status()
    {
        return Ok(new
        {
            authenticated = User.Identity?.IsAuthenticated ?? false,
            mode = _authOptions.Mode,
            user = User.Identity?.Name
        });
    }
}
