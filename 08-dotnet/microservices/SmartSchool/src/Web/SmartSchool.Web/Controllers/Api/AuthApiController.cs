using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UserManagement.Application.DTOs;
using UserManagement.Domain.Entities;
using UserManagement.Persistence;

namespace SmartSchool.Web.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class AuthApiController : ControllerBase
{
    private readonly UserManagementDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthApiController(UserManagementDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto model)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.UserName == model.UserName && !u.IsDeleted);

        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
        {
            return Unauthorized(new LoginResponseDto
            {
                Success = false,
                Message = "Invalid username or password"
            });
        }

        if (!user.IsActive)
        {
            return Unauthorized(new LoginResponseDto
            {
                Success = false,
                Message = "Account is deactivated"
            });
        }

        var token = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(
            int.Parse(_configuration["JwtSettings:RefreshTokenExpiryDays"]!));
        user.LastLoginAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();

        return Ok(new LoginResponseDto
        {
            Success = true,
            Token = token,
            RefreshToken = refreshToken,
            TokenExpiry = DateTime.UtcNow.AddMinutes(
                double.Parse(_configuration["JwtSettings:TokenExpiryMinutes"]!)),
            User = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                TenantId = user.TenantId,
                Roles = user.UserRoles.Select(ur => ur.Role?.Name ?? "").Where(r => !string.IsNullOrEmpty(r))
            }
        });
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponseDto>> RefreshToken([FromBody] RefreshTokenDto model)
    {
        var principal = GetPrincipalFromExpiredToken(model.Token);
        if (principal == null)
        {
            return Unauthorized(new LoginResponseDto { Success = false, Message = "Invalid token" });
        }

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new LoginResponseDto { Success = false, Message = "Invalid token" });
        }

        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

        if (user == null || user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiry <= DateTime.UtcNow)
        {
            return Unauthorized(new LoginResponseDto { Success = false, Message = "Invalid refresh token" });
        }

        var newToken = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(
            int.Parse(_configuration["JwtSettings:RefreshTokenExpiryDays"]!));
        
        await _context.SaveChangesAsync();

        return Ok(new LoginResponseDto
        {
            Success = true,
            Token = newToken,
            RefreshToken = newRefreshToken,
            TokenExpiry = DateTime.UtcNow.AddMinutes(
                double.Parse(_configuration["JwtSettings:TokenExpiryMinutes"]!))
        });
    }

    private string GenerateJwtToken(ApplicationUser user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        if (user.TenantId.HasValue)
            claims.Add(new Claim("TenantId", user.TenantId.Value.ToString()));

        foreach (var userRole in user.UserRoles)
        {
            if (userRole.Role != null)
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["TokenExpiryMinutes"]!)),
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateLifetime = false,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }
            return principal;
        }
        catch
        {
            return null;
        }
    }
}
