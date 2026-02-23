using AppBuilder.Core.Interfaces;
using AppBuilder.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppBuilder.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly ILogger<AuthController> _log;

    public AuthController(IAuthService auth, ILogger<AuthController> log)
    {
        _auth = auth;
        _log = log;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest r)
    {
        if (string.IsNullOrWhiteSpace(r.Email) || string.IsNullOrWhiteSpace(r.Password))
            return BadRequest(new { error = "Email and password required" });
        try
        {
            var (user, token) = _auth.Register(r.Email, r.Password, r.Name);
            return Ok(new { user = SanitizeUser(user), token });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest r)
    {
        if (string.IsNullOrWhiteSpace(r.Email) || string.IsNullOrWhiteSpace(r.Password))
            return BadRequest(new { error = "Email and password required" });
        var (user, token) = _auth.Login(r.Email, r.Password);
        if (user == null) return Unauthorized(new { error = "Invalid email or password" });
        return Ok(new { user = SanitizeUser(user), token });
    }

    [HttpPost("social")]
    public IActionResult SocialLogin([FromBody] SocialLoginRequest r)
    {
        if (string.IsNullOrWhiteSpace(r.Provider) || string.IsNullOrWhiteSpace(r.ProviderId) || string.IsNullOrWhiteSpace(r.Email))
            return BadRequest(new { error = "Provider, ProviderId, and Email required" });
        var (user, token) = _auth.SocialLogin(r.Provider, r.ProviderId, r.Email, r.Name, r.AvatarUrl);
        if (user == null) return BadRequest(new { error = "Social login failed" });
        return Ok(new { user = SanitizeUser(user), token });
    }

    static object SanitizeUser(User u) => new { u.Id, u.Email, u.Name, u.AvatarUrl, u.PlanTier, u.BuildCredits, u.CreatedAt };
}

public class RegisterRequest { public string Email { get; set; } = ""; public string Password { get; set; } = ""; public string? Name { get; set; } }
public class LoginRequest { public string Email { get; set; } = ""; public string Password { get; set; } = ""; }
public class SocialLoginRequest { public string Provider { get; set; } = ""; public string ProviderId { get; set; } = ""; public string Email { get; set; } = ""; public string? Name { get; set; } public string? AvatarUrl { get; set; } }
