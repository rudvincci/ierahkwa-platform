using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Mamey.Government.Modules.CitizenshipApplications.Core.Services;

internal class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtTokenService> _logger;

    public JwtTokenService(IConfiguration configuration, ILogger<JwtTokenService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public Task<string> GenerateApplicantTokenAsync(string email, Guid applicationId, CancellationToken cancellationToken = default)
    {
        var secretKey = _configuration["Jwt:SecretKey"] ?? _configuration["JWT_SECRET_KEY"] ?? "default-secret-key-change-in-production-minimum-32-characters";
        var issuer = _configuration["Jwt:Issuer"] ?? _configuration["JWT_ISSUER"] ?? "Mamey.Government";
        var audience = _configuration["Jwt:Audience"] ?? _configuration["JWT_AUDIENCE"] ?? "Mamey.Government";
        var expirationHours = int.Parse(_configuration["Jwt:ExpirationHours"] ?? _configuration["JWT_EXPIRATION_HOURS"] ?? "24");

        if (secretKey.Length < 32)
        {
            _logger.LogWarning("JWT secret key is too short. Using default key. Please configure a secure key in production.");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Email, email),
            new Claim("applicationId", applicationId.ToString()),
            new Claim(ClaimTypes.Role, "Applicant"),
            new Claim(ClaimTypes.NameIdentifier, email), // Use email as identifier
            new Claim(JwtRegisteredClaimNames.Sub, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expirationHours),
            signingCredentials: credentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenString = tokenHandler.WriteToken(token);

        return Task.FromResult(tokenString);
    }
}
