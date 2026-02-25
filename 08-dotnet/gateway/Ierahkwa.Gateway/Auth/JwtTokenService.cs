using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Ierahkwa.Gateway.Auth;

public class JwtTokenService
{
    private readonly string _key;
    private readonly string _issuer;
    private readonly SymmetricSecurityKey _signingKey;

    public JwtTokenService(string key, string issuer)
    {
        _key = key;
        _issuer = issuer;
        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
    }

    public string GenerateToken(string userId, string tenantId, string tier, string[] roles)
    {
        var claims = new List<Claim>
        {
            new("sub", userId),
            new("tenant", tenantId),
            new("tier", tier),
            new("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        foreach (var role in roles)
            claims.Add(new Claim("role", role));

        var creds = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: null,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        try
        {
            var principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                IssuerSigningKey = _signingKey,
                ClockSkew = TimeSpan.FromMinutes(2)
            }, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    public string GenerateTokenFromPrincipal(ClaimsPrincipal principal)
    {
        var userId = principal.FindFirst("sub")?.Value ?? "";
        var tenantId = principal.FindFirst("tenant")?.Value ?? "";
        var tier = principal.FindFirst("tier")?.Value ?? "member";
        var roles = principal.FindAll("role").Select(c => c.Value).ToArray();
        return GenerateToken(userId, tenantId, tier, roles);
    }
}
