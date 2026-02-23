using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Mamey.Identity.Core;
using Mamey.Identity.Distributed.Configuration;
using Mamey.Identity.Redis.Services;

namespace Mamey.Identity.Distributed.Services;

/// <summary>
/// Service for managing distributed tokens across microservices.
/// </summary>
public class DistributedTokenService : IDistributedTokenService
{
    private readonly ILogger<DistributedTokenService> _logger;
    private readonly DistributedIdentityOptions _options;
    private readonly IRedisTokenCache _tokenCache;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public DistributedTokenService(
        ILogger<DistributedTokenService> logger,
        IOptions<DistributedIdentityOptions> options,
        IRedisTokenCache tokenCache)
    {
        _logger = logger;
        _options = options.Value;
        _tokenCache = tokenCache;
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    public async Task<string> CreateDistributedTokenAsync(AuthenticatedUser user, string serviceId, CancellationToken cancellationToken = default)
    {
        try
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Name ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim("service_id", serviceId),
                new Claim("user_type", user.Type ?? "User"),
                new Claim("tenant_id", user.TenantId?.ToString() ?? string.Empty),
                new Claim("jti", Guid.NewGuid().ToString()),
                new Claim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            // Add custom claims from user
            if (user.Claims != null)
            {
                foreach (var claim in user.Claims)
                {
                    claims.Add(new Claim(claim.Key, claim.Value));
                }
            }

            var key = new SymmetricSecurityKey(Convert.FromBase64String(_options.JwtSigningKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _options.JwtIssuer,
                audience: _options.JwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_options.JwtExpirationMinutes),
                signingCredentials: credentials
            );

            var tokenString = _tokenHandler.WriteToken(token);

            // Cache the token
            var cacheKey = $"distributed_token:{user.UserId}:{serviceId}";
            await _tokenCache.SetCachedTokenAsync(cacheKey, tokenString);

            _logger.LogDebug("Created distributed token for user {UserId} and service {ServiceId}", user.UserId, serviceId);
            return tokenString;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating distributed token for user {UserId} and service {ServiceId}", user.UserId, serviceId);
            throw;
        }
    }

    public async Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
                return false;

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = _options.JwtValidationParameters;

            try
            {
                tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                
                // Check if token is in cache (not revoked)
                var jti = GetTokenJti(token);
                if (!string.IsNullOrEmpty(jti))
                {
                    var cacheKey = $"distributed_token:{jti}";
                    var cachedToken = await _tokenCache.GetCachedTokenAsync(cacheKey);
                    if (string.IsNullOrEmpty(cachedToken))
                    {
                        _logger.LogWarning("Token {Jti} not found in cache, may be revoked", jti);
                        return false;
                    }
                }

                return true;
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning(ex, "Token validation failed");
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            return false;
        }
    }

    public async Task<string> RefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await ValidateTokenAsync(token, cancellationToken))
            {
                throw new InvalidOperationException("Cannot refresh invalid token");
            }

            var user = await GetUserFromTokenAsync(token, cancellationToken);
            if (user == null)
            {
                throw new InvalidOperationException("Cannot refresh token without user information");
            }

            var serviceId = GetTokenServiceId(token);
            return await CreateDistributedTokenAsync(user, serviceId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            throw;
        }
    }

    public async Task<bool> RevokeTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            var jti = GetTokenJti(token);
            if (string.IsNullOrEmpty(jti))
                return false;

            var cacheKey = $"distributed_token:{jti}";
            await _tokenCache.SetCachedTokenAsync(cacheKey, string.Empty); // Set to empty to revoke

            _logger.LogDebug("Revoked token {Jti}", jti);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking token");
            return false;
        }
    }

    public async Task<IEnumerable<Claim>> GetClaimsAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!await ValidateTokenAsync(token, cancellationToken))
            {
                return Enumerable.Empty<Claim>();
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.Claims;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting claims from token");
            return Enumerable.Empty<Claim>();
        }
    }

    public async Task<AuthenticatedUser?> GetUserFromTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            var claims = await GetClaimsAsync(token, cancellationToken);
            if (!claims.Any())
                return null;

            var claimsList = claims.ToList();
            var userIdClaim = claimsList.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var nameClaim = claimsList.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            var emailClaim = claimsList.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            var typeClaim = claimsList.FirstOrDefault(c => c.Type == "user_type");
            var tenantIdClaim = claimsList.FirstOrDefault(c => c.Type == "tenant_id");

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
                return null;

            var user = new AuthenticatedUser
            {
                UserId = userId,
                Name = nameClaim?.Value,
                Email = emailClaim?.Value,
                Type = typeClaim?.Value,
                TenantId = Guid.TryParse(tenantIdClaim?.Value, out var tenantId) ? tenantId : null,
                Claims = claimsList.ToDictionary(c => c.Type, c => c.Value)
            };

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user from token");
            return null;
        }
    }

    private string GetTokenJti(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.Claims.FirstOrDefault(c => c.Type == "jti")?.Value ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    private string GetTokenServiceId(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            return jwtToken.Claims.FirstOrDefault(c => c.Type == "service_id")?.Value ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}


































