using Microsoft.AspNetCore.Mvc;
using AdvocateOffice.Services;

namespace AdvocateOffice.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly DbService _db;

    public AuthController(DbService db) => _db = db;

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest req)
    {
        if (string.IsNullOrEmpty(req.Username) || string.IsNullOrEmpty(req.Password))
            return BadRequest(new { error = "Username and password required" });

        var user = _db.FindUserByUsername(req.Username);
        if (user == null || !_db.VerifyPassword(user.PasswordHash, req.Password))
            return Unauthorized(new { error = "Invalid credentials" });

        var token = TokenStore.Create(user.Id);
        return Ok(new
        {
            success = true,
            token,
            user = new { id = user.Id, username = user.Username, fullName = user.FullName, role = user.Role }
        });
    }

    [HttpPost("logout")]
    public IActionResult Logout([FromHeader(Name = "Authorization")] string? auth)
    {
        var t = auth?.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase).Trim();
        TokenStore.Remove(t);
        return Ok(new { success = true });
    }

    [HttpGet("check")]
    public IActionResult Check([FromHeader(Name = "Authorization")] string? auth)
    {
        if (!TokenStore.Validate(auth?.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase).Trim(), out var uid))
            return Ok(new { authenticated = false });

        var user = _db.GetUserById(uid);
        if (user == null) return Ok(new { authenticated = false });

        return Ok(new
        {
            authenticated = true,
            user = new { id = user.Id, username = user.Username, fullName = user.FullName, role = user.Role }
        });
    }
}

public class LoginRequest
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
}
